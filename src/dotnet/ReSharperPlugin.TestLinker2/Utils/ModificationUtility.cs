using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.DocumentManagers.impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.ReSharper.UnitTestFramework.Features;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker2.Utils;

public static class ModificationUtility
{
	public static void TryCreateTestOrProductionClass(ITypeElement sourceType, ITextControl textControl)
	{
		var solution = sourceType.GetSolution();

		var typesNearCaretOrFile = solution.GetComponent<ITypesFromTextControlService>()
			.GetTypesNearCaretOrFile(textControl, solution);
		var templateTypes = typesNearCaretOrFile.Select(GetLinkedTypeWithDerivedName).WhereNotNull()
			.FirstOrDefault();

		if (templateTypes == null)
		{
			MessageBox.ShowInfo(
				"Could not find a template to create production/test class from.\r\n" +
				"There must exist at least one pair of production+test classes for this project.");

			return;
		}

		var templateSourceType = templateTypes.Item1;
		var templateLinkedType = templateTypes.Item2;

		var linkedTypeName = DerivedNameUtility.GetDerivedName(sourceType.ShortName, templateSourceType.ShortName,
			templateLinkedType.ShortName);
		var linkedTypeNamespace = DerivedNameUtility.GetDerivedNamespace(sourceType, templateLinkedType);
		var linkedTypeSourceFile = templateLinkedType.GetSingleOrDefaultSourceFile()
			.NotNull("linkedTypeSourceFile != null");
		var linkedTypeProject = linkedTypeSourceFile.GetProject().NotNull("linkedTypeProject != null");
		var linkedTypeKind = !solution.GetComponent<IUnitTestPsiManager>()
			.IsElementOfKind(templateLinkedType, UnitTestElementKind.TestContainer)
			? TypeKind.Production
			: TypeKind.Test;

		var rootNamespace = linkedTypeProject.GetDefaultNamespace() ?? linkedTypeProject.Name;
		var shortenedLinkedTypeNamespace = linkedTypeNamespace
			.TrimFromStart(rootNamespace)
			.TrimFromStart(".");

		if (!MessageBox.ShowYesNo(
			    $"Class: {linkedTypeName}\r\nProject: {linkedTypeProject.Name}\r\nNamespace: {shortenedLinkedTypeNamespace}\r\n",
			    $"Create {linkedTypeKind} class for {sourceType.ShortName}?"))
			return;

		var threading = solution.GetComponent<IThreading>();
		threading.ExecuteOrQueueEx(
			nameof(TryCreateTestOrProductionClass),
			() =>
			{
				var linkedTypeProjectFolder = GetLinkedTypeFolder(linkedTypeNamespace, linkedTypeProject);
				var linkedTypeFile = GetLinkedTypeFile(linkedTypeName, linkedTypeNamespace, templateLinkedType);
				var linkedTypeProjectFile = AddNewItemHelper.AddFile(linkedTypeProjectFolder,
					linkedTypeName + ".cs", linkedTypeFile.GetText());
				linkedTypeProjectFile.Navigate(Shell.Instance.GetComponent<IMainWindowPopupWindowContext>().Source,
					true);
			});
	}

	[CanBeNull]
	private static Tuple<ITypeElement, ITypeElement> GetLinkedTypeWithDerivedName(ITypeElement sourceType)
	{
		return LinkedTypesUtil.GetLinkedTypes(sourceType)
			.Where(x => DerivedNameUtility.IsDerivedNameAny(sourceType.ShortName, x.ShortName))
			.Select(x => Tuple.Create(sourceType, x))
			.FirstOrDefault();
	}

	private static ICSharpFile GetLinkedTypeFile(string linkedTypeName, string linkedTypeNamespace,
		ITypeElement templateLinkedType)
	{
		var elementFactory = CSharpElementFactory.GetInstance(
			templateLinkedType.GetFirstDeclaration<IDeclaration>().NotNull());

		var templateLinkedTypeSourceFile = templateLinkedType.GetSingleOrDefaultSourceFile()
			.NotNull("templateLinkedTypeSourceFile != null");

		var templateFile = templateLinkedTypeSourceFile.GetPrimaryPsiFile().NotNull("templateFile != null");

		var fileText = templateFile.GetText()
			.Replace(templateLinkedType.GetContainingNamespace().QualifiedName, linkedTypeNamespace)
			.Replace(templateLinkedType.ShortName, linkedTypeName);

		var linkedTypeFile = elementFactory.CreateFile(fileText);

		var typeDeclarations = GetTypeDeclarations(linkedTypeFile);
		var linkedType = (IClassDeclaration) typeDeclarations.Single(x => x.DeclaredName == linkedTypeName);

		// Remove base types
		foreach (var x in linkedType.SuperTypes)
			linkedType.RemoveSuperInterface(x);

		// Clear body
		linkedType.SetBody(((IClassLikeDeclaration) elementFactory.CreateTypeMemberDeclaration("class C{}")).Body);

		// Remove unrelated types
		foreach (var declaration in linkedTypeFile.TypeDeclarations.Where(x => x.DeclaredName != linkedTypeName))
			ModificationUtil.DeleteChild(declaration);

		return linkedTypeFile;
	}

	private static IProjectFolder GetLinkedTypeFolder(string linkedTypeNamespace, IProject linkedTypeProject)
	{
		var rootNamespace = linkedTypeProject.GetDefaultNamespace() ?? linkedTypeProject.Name;
		var linkedTypeFolder = linkedTypeNamespace.TrimFromStart(rootNamespace)
			.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries)
			.Aggregate(linkedTypeProject.Location,
				(currentFolder, nextFolder) => currentFolder.Combine(nextFolder));

		return linkedTypeProject.GetOrCreateProjectFolder(linkedTypeFolder).NotNull();
	}

	private static IReadOnlyCollection<ITypeDeclaration> GetTypeDeclarations(ICSharpFile csharpFile)
	{
		var namespaceDeclarations =
			csharpFile.NamespaceDeclarations.SelectMany(x => x.DescendantsAndSelf(y => y.NamespaceDeclarations));
		return namespaceDeclarations.Cast<ITypeDeclarationHolder>()
			.SelectMany(x => x.TypeDeclarations)
			.SelectMany(x => x.DescendantsAndSelf(y => y.TypeDeclarations))
			.ToList();
	}
}