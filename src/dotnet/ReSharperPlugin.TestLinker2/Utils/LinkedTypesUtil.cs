using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;
using JetBrains.Util;
using ReSharperPlugin.TestLinker2.Options;

namespace ReSharperPlugin.TestLinker2.Utils;

public static class LinkedTypesUtil
{
	public static IReadOnlyCollection<ITypeElement> GetLinkedTypes(IDataContext dataContext)
	{
		ISolution solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION);
		ITextControl textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);

		if (solution == null || textControl == null)
		{
			return Array.Empty<ITypeElement>();
		}

		ITypesFromTextControlService typesInContextProvider =
			dataContext.GetComponent<ITypesFromTextControlService>().NotNull();

		IEnumerable<ITypeElement> typesInContext =
			typesInContextProvider.GetTypesFromCaretOrFile(textControl, solution);
		return typesInContext.SelectMany(GetLinkedTypes).ToList();
	}

	public static IReadOnlyCollection<ITypeElement> GetLinkedTypes(ITypeElement source)
	{
		IEnumerable<ITypeElement> sources = new[] {source}
			.Concat(source.GetAllSuperTypes()
				.Select(x => x.GetTypeElement())
				.WhereNotNull()
				.Where(x => !x.IsObjectClass()));

		List<ITypeElement> linkedTypes = sources.SelectMany(GetLinkedTypesInternal).ToList();

		IPsiServices services = source.GetPsiServices();

		// Insert All Types that Inherit From Each Linked Type
		foreach (ITypeElement linkedType in linkedTypes)
		{
			var findResultConsumer = new FindResultConsumer(inheritorResult =>
			{
				if ((inheritorResult as FindResultDeclaredElement)?.DeclaredElement is ITypeElement typeElement)
					linkedTypes.Add(typeElement);

				return FindExecution.Continue;
			});
			services.Finder.FindInheritors(linkedType, findResultConsumer, NullProgressIndicator.Create());
		}

		return linkedTypes;
	}

	private static IReadOnlyCollection<ITypeElement> GetLinkedTypesInternal(ITypeElement source)
	{
		TestLinkerSettings settings = source.GetSolution().GetSettings();
		string[] derivedNames = GetDerivedNames(source, settings.GetNamingSuffixesArray());

		string attributeName = settings.TypeofAttributeName.TrimFromEnd("Attribute");

		IEnumerable<ITypeElement> types = GetAttributeLinkedTypes(source, attributeName);
		if (types != null)
			return types.ToList();

		IPsiServices psiServices = source.GetPsiServices();

		ISymbolScope symbolCache = psiServices.Symbols.GetSymbolScope(LibrarySymbolScope.NONE, true);
		List<ITypeElement> linkedTypes = new();

		// Handle Link By Name
		IEnumerable<ITypeElement> linkedTypesByName = derivedNames
			.SelectMany(derivedName => symbolCache.GetElementsByShortName(derivedName))
			.OfType<ITypeElement>();

		linkedTypes.AddRange(linkedTypesByName);

		// Handling Link By Something With The Attribute
		IWordIndex wordIndex = psiServices.WordIndex;
		IEnumerable<IPsiSourceFile> sourceFiles = wordIndex.GetFilesContainingAllWords(new[] {source.ShortName});
		IEnumerable<ClassLikeTypeElement> typesInFiles = sourceFiles
			.SelectMany(psiServices.Symbols.GetTypesAndNamespacesInFile)
			.OfType<ClassLikeTypeElement>()
			.Where(x => GetAttributeLinkedTypes(x, attributeName)?.Contains(source) ?? false);
		linkedTypes.AddRange(typesInFiles);

		return linkedTypes.Where(x => !x.Equals(source)).ToList();
	}

	[CanBeNull]
	private static IEnumerable<ITypeElement> GetAttributeLinkedTypes(IAttributesSet attributesSet, string attributeName)
	{
		IAttributeInstance attribute = GetFirstAttributeInstanceThatStartsWith(attributesSet, attributeName);
		if (attribute == null)
			return null;

		IEnumerable<AttributeValue> namedArguments = attribute.NamedParameters().Select(x => x.Second);
		IEnumerable<AttributeValue> positionalArguments = attribute.PositionParameters();
		IEnumerable<AttributeValue> flattenedArguments = FlattenArguments(namedArguments.Concat(positionalArguments));

		return flattenedArguments
			.Where(value => value.IsType && !value.IsBadValue)
			.Select(value => value.TypeValue.GetTypeElement())
			.WhereNotNull();
	}

	private static IAttributeInstance GetFirstAttributeInstanceThatStartsWith(
		IAttributesSet attributesSet,
		string attributeName)
	{
		return attributesSet.GetAttributeInstances(true)
			.SingleOrDefault(attributeInstance =>
				attributeInstance.GetAttributeShortName()?.StartsWith(attributeName) ?? false);
	}

	private static string[] GetDerivedNames(ITypeElement source, string[] suffixes)
	{
		string shortName = source.ShortName;
		return suffixes.Any(suffix => shortName.StartsWith(suffix) || shortName.EndsWith(suffix))
			? new[] {
				suffixes.Aggregate(shortName, (name, suffix) => name.TrimFromStart(suffix).TrimFromEnd(suffix))
			}
			: suffixes.SelectMany(suffix => new[] {shortName + suffix, suffix + shortName}).ToArray();
	}


	private static IEnumerable<AttributeValue> FlattenArguments(IEnumerable<AttributeValue> attributeValues)
	{
		foreach (AttributeValue attributeValue in attributeValues)
		{
			if (!attributeValue.IsArray)
			{
				yield return attributeValue;
			}
			else
			{
				foreach (AttributeValue innerAttributeValue in FlattenArguments(attributeValue.ArrayValue.NotNull()))
				{
					yield return innerAttributeValue;
				}
			}
		}
	}
}