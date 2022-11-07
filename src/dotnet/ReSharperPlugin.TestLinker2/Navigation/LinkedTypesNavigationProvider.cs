using JetBrains.Application;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Tooltips;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker2.Actions;

namespace ReSharperPlugin.TestLinker2.Navigation;

[ContextNavigationProvider]
public class LinkedTypesNavigationProvider
	: LinkedTypesNavigationProviderBase<LinkedTypesContextSearch>
{
	public LinkedTypesNavigationProvider(
		IShellLocks locks,
		ITooltipManager tooltipManager,
		IFeaturePartsContainer manager
	) : base(manager)
	{
	}

	protected override string ActionId => GotoLinkedTypes2Action.Id;

	protected override string NavigationMenuTitle => "Linked Types";
}