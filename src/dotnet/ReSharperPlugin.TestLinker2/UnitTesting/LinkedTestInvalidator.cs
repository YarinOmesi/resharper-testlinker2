using System.Collections.Generic;
using System.Linq;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;
using ReSharperPlugin.TestLinker2.Utils;

namespace ReSharperPlugin.TestLinker2.UnitTesting
{
	[PsiComponent]
	internal class LinkedTestInvalidator
	{
		private readonly IUnitTestElementStuff _unitTestElementStuff;
		private readonly IUnitTestResultManager _unitTestResultManager;

		public LinkedTestInvalidator(
			Lifetime lifetime,
			ChangedTypesProvider changedTypesProvider,
			IUnitTestElementStuff unitTestElementStuff,
			IUnitTestResultManager unitTestResultManager)
		{
			_unitTestElementStuff = unitTestElementStuff;
			_unitTestResultManager = unitTestResultManager;

			changedTypesProvider.TypesChanged.Advise(lifetime, OnChanged);
		}

		private void OnChanged(IReadOnlyCollection<ITypeElement> changedTypes)
		{
			using (CompilationContextCookie.GetExplicitUniversalContextIfNotSet())
			{
				var linkedTypes = changedTypes.SelectMany(LinkedTypesUtil.GetLinkedTypes).ToList();
				var relevantTests = linkedTypes.Select(_unitTestElementStuff.GetElement).WhereNotNull();
				foreach (var relevantTest in relevantTests)
					_unitTestResultManager.MarkOutdated(relevantTest);
			}
		}
	}
}
