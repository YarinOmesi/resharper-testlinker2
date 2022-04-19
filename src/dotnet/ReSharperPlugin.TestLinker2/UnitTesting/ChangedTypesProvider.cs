using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.changes;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.DocumentManagers;
using JetBrains.DocumentManagers.impl;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Threading;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker2.UnitTesting
{
	[PsiComponent]
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "TypesChanged is disposed via Lifetime")]
	internal class ChangedTypesProvider
	{
		// ReSharper disable once InconsistentNaming
		private static readonly TimeSpan s_updateInterval = TimeSpan.FromMilliseconds(1000);

		private readonly ConcurrentDictionary<IProjectFile, TextRange> _myChangedRanges;
		private readonly GroupingEvent _myDocumentChangedEvent;
		private readonly DocumentManager _myDocumentManager;

		private readonly Lifetime _myLifetime;
		private readonly IPsiServices _myServices;
		private readonly IShellLocks _myShellLocks;

		public ChangedTypesProvider(
			Lifetime lifetime,
			IShellLocks shellLocks,
			ChangeManager changeManager,
			DocumentManager documentManager,
			IPsiServices services)
		{
			_myServices = services;
			_myLifetime = lifetime;
			_myShellLocks = shellLocks;
			_myDocumentManager = documentManager;
			_myChangedRanges = new ConcurrentDictionary<IProjectFile, TextRange>();

			changeManager.Changed2.Advise(lifetime, OnChange);
			_myDocumentChangedEvent = shellLocks.CreateGroupingEvent(
				lifetime,
				"ChangedTypesProvider::DocumentChanged",
				s_updateInterval,
				OnProcessChangesEx);

			TypesChanged = new Signal<IReadOnlyCollection<ITypeElement>>(lifetime, "ChangedTypesProvider");
			_myLifetime.AddDispose(TypesChanged);
		}

		public ISignal<IReadOnlyCollection<ITypeElement>> TypesChanged { get; }

		private void OnChange(ChangeEventArgs e)
		{
			var change = e.ChangeMap.GetChange<ProjectFileDocumentChange>(_myDocumentManager.ChangeProvider);
			if (change == null)
				return;

			lock (_myChangedRanges)
			{
				var changeRange = new TextRange(change.StartOffset, change.StartOffset + change.OldLength);
				_myChangedRanges.AddOrUpdate(change.ProjectFile, changeRange, (file, range) => changeRange.Join(range));
			}

			_myDocumentChangedEvent.FireIncoming();
		}

		private void OnProcessChangesEx()
		{
			using (_myShellLocks.UsingReadLock())
			{
				_myServices.Files.CommitAllDocumentsAsync(
					() =>
					{
						var the = new InterruptableReadActivityThe(_myLifetime, _myShellLocks, new InterruptionSet())
							{FuncRun = Invalidate};
						the.DoStart();
					},
					() => { });
			}
		}

		private void Invalidate()
		{
			var changes = GetChanges();

			try
			{
				var allChangedTypes = new List<ITypeElement>();

				foreach (var changedRange in changes)
				{
					var sourceFile = changedRange.Key.ToSourceFile();
					if (sourceFile == null)
					{
						continue;
					}

					var containedTypes = _myServices.Symbols.GetTypesAndNamespacesInFile(sourceFile)
						.OfType<ITypeElement>().Where(x => x.IsValid());

					var changedTypes = containedTypes.Where(
						x => x.GetDeclarationsIn(sourceFile)
							.Select(t => t.GetDocumentRange().TextRange).Where(t => t.IsValid)
							.Any(t => changedRange.Value.ContainedIn(t)));

					allChangedTypes.AddRange(changedTypes);
				}

				TypesChanged.Fire(allChangedTypes);
			}
			catch (OperationCanceledException)
			{
				ReAddChanges(changes);
			}
		}

		private KeyValuePair<IProjectFile, TextRange>[] GetChanges()
		{
			KeyValuePair<IProjectFile, TextRange>[] changes;

			lock (_myChangedRanges)
			{
				changes = _myChangedRanges.Where(x => x.Key.IsValid() && x.Value.IsValid).ToArray();
				_myChangedRanges.Clear();
			}

			return changes;
		}

		private void ReAddChanges(KeyValuePair<IProjectFile, TextRange>[] changes)
		{
			lock (_myChangedRanges)
			{
				foreach (var pair in changes)
				{
					_myChangedRanges.AddOrUpdate(pair.Key, pair.Value, (file, range) => pair.Value.Join(range));
				}
			}
		}
	}
}
