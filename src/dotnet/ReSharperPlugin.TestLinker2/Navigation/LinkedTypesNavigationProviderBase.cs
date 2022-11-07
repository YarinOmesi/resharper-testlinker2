using System;
using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Navigation.ExecutionHosting;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Features.Navigation.Features.FindHierarchy;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace ReSharperPlugin.TestLinker2.Navigation;

public abstract class LinkedTypesNavigationProviderBase<T> :
	HierarchyProviderBase<T, LinkedTypesSearchRequest, LinkedTypesSearchDescriptor>,
	INavigateFromHereProvider where T : class, IRequestContextSearch
{
	private readonly ILogger _logger;

	protected LinkedTypesNavigationProviderBase(IFeaturePartsContainer manager)
		: base(manager)
	{
		_logger = Logger.GetLogger<LinkedTypesNavigationProviderBase<T>>();
	}

	protected abstract string ActionId { get; }
	protected abstract string NavigationMenuTitle { get; }

	public IEnumerable<ContextNavigation> CreateWorkflow(IDataContext dataContext)
	{
		_logger.Info($"CreateWorkflow");
		ISolution solution = dataContext.GetData(ProjectModelDataConstants.SOLUTION);
		INavigationExecutionHost navigationExecutionHost = DefaultNavigationExecutionHost.GetInstance(solution);

		Action execution = GetSearchesExecution(dataContext, navigationExecutionHost);
		if (execution == null)
			yield break;

		yield return new ContextNavigation(
			NavigationMenuTitle,
			ActionId,
			NavigationActionGroup.Important,
			execution
		);
	}

	public override string GetNotFoundMessage(SearchRequest request) => "No linked types found";

	protected override OccurrencePresentationOptions ProvideFeatureSpecificPresentationOptions(
		LinkedTypesSearchRequest searchRequest)
	{
		_logger.Info("ProvideFeatureSpecificPresentationOptions");
		return new OccurrencePresentationOptions();
	}

	protected override LinkedTypesSearchDescriptor CreateSearchDescriptor(
		LinkedTypesSearchRequest searchRequest,
		ICollection<IOccurrence> results
	)
	{
		_logger.Info("CreateSearchDescriptor");
		return new LinkedTypesSearchDescriptor(searchRequest, results);
	}
}