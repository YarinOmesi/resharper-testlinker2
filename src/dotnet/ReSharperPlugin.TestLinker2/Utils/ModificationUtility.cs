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
		ISolution solution = sourceType.GetSolution();

		IEnumerable<ITypeElement> typesNearCaretOrFile = solution.GetComponent<ITypesFromTextControlService>()
			.GetTypesNearCaretOrFile(textControl, solution);
		Tuple<ITypeElement, ITypeElement> templateTypes = typesNearCaretOrFile.Select(GetLinkedTypeWithDerivedName)
			.WhereNotNull()
			.FirstOrDefault();

		if (templateTypes == null)
		{
			MessageBox.ShowInfo(
				"Could not find a template to create production/test class from.\r\n" +
				"There must exist at least one pair of production+test classes for this project.");

			return;
		}

		ITypeElement templateSourceType = templateTypes.Item1;
		ITypeElement templateLinkedType = templateTypes.Item2;

		string linkedTypeName = DerivedNameUtility.GetDerivedName(
			sourceType.ShortName,
			templateSourceType.ShortName,
			templateLinkedType.ShortName
			);

		string linkedTypeNamespace = DerivedNameUtility.GetDerivedNamespace(sourceType, templateLinkedType);

		IPsiSourceFile linkedTypeSourceFile = templateLinkedType.GetSingleOrDefaultSourceFile()
			.NotNull("linkedTypeSourceFile != null");

		IProject linkedTypeProject = linkedTypeSourceFile.GetProject().NotNull("linkedTypeProject != null");
		TypeKind linkedTypeKind = !solution.GetComponent<IUnitTestPsiManager>()
			.IsElementOfKind(templateLinkedType, UnitTestElementKind.TestContainer)
			? TypeKind.Production
			: TypeKind.Test;

		string rootNamespace = linkedTypeProject.GetDefaultNamespace() ?? linkedTypeProject.Name;
		string shortenedLinkedTypeNamespace = linkedTypeNamespace
			.TrimFromStart(rootNamespace)
			.TrimFromStart(".");

		bool userWantNewClass = AskUseIfCreateNewClass(
			sourceType.ShortName,
			linkedTypeName,
			linkedTypeProject.Name,
			shortenedLinkedTypeNamespace,
			linkedTypeKind
		);
		if (!userWantNewClass)
		{
			return;
		}

		// Creating New Test File
		var threading = solution.GetComponent<IThreading>();
		threading.ExecuteOrQueueEx(nameof(TryCreateTestOrProductionClass), () =>
		{
			IProjectFolder linkedTypeProjectFolder = GetLinkedTypeFolder(linkedTypeNamespace, linkedTypeProject);
			ICSharpFile linkedTypeFile = GetLinkedTypeFile(linkedTypeName, linkedTypeNamespace, templateLinkedType);
			IProjectFile linkedTypeProjectFile = AddNewItemHelper.AddFile(linkedTypeProjectFolder,
				linkedTypeName + ".cs", linkedTypeFile.GetText());
			linkedTypeProjectFile.Navigate(Shell.Instance.GetComponent<IMainWindowPopupWindowContext>().Source, true);
		});
	}

	private static bool AskUseIfCreateNewClass(
		string sourceName,
		string newClassName,
		string newClassProjectName,
		string newClassNamespace,
		TypeKind linkedTypeKind)
	{
		var text = $"Class: {newClassName}\r\nProject: {newClassProjectName}\r\nNamespace: {newClassNamespace}\r\n";
		var caption = $"Create {linkedTypeKind} class for {sourceName}?";

		return MessageBox.ShowYesNo(text, caption);
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