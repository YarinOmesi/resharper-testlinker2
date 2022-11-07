using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Diagrams.TypeDependencies;
using JetBrains.ReSharper.Feature.Services.Navigation.Descriptors;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Feature.Services.Tree.SectionsManagement;
using JetBrains.Util;

namespace ReSharperPlugin.TestLinker2.Navigation;

// TODO: what is it good for?
public sealed class LinkedTypesSearchDescriptor : SearchDescriptor
{
	public override string ActionBarID => "TreeModelBrowser.Standard";

	public override TypeDependenciesOptions DiagrammingOptions => new(
		new[] {TypeElementDependencyType.ReturnType},
		TypeDependenciesOptions.CollapseBigFoldersFunc
	);

	private const string NoLinkedTypesFoundText = "No linked types found";

	public LinkedTypesSearchDescriptor(LinkedTypesSearchRequest request, ICollection<IOccurrence> results)
		: base(request, results)
	{
	}

	public override string GetResultsTitle(OccurrenceSection section)
	{
		if (section.TotalCount == 0)
		{
			myLogger.Info("GetResultsTitle: No Linked Types Found");
			return NoLinkedTypesFoundText;
		}

		string typeOrTypes = NounUtil.ToPluralOrSingular("type", section.TotalCount);
		return section.FilteredCount == section.TotalCount
			? $"Found {section.TotalCount} linked {typeOrTypes}"
			: $"Displaying {section.FilteredCount} of {section.TotalCount} linked {typeOrTypes}";
	}

	protected override Func<SearchRequest, IOccurrenceBrowserDescriptor> GetDescriptorFactory() => DescriptorFactory;
	private static IOccurrenceBrowserDescriptor DescriptorFactory(SearchRequest request)
	{
		var declarationRequest = (LinkedTypesSearchRequest) request;
		ICollection<IOccurrence> results = request.Search();
		return new LinkedTypesSearchDescriptor(declarationRequest, results);
	}

}