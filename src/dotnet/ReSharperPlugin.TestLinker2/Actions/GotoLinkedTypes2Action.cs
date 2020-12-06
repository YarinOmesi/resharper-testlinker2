using JetBrains.Application.DataContext;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.Application.UI.ActionSystem.ActionsRevised.Menu;
using JetBrains.ReSharper.Feature.Services.Actions;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using ReSharperPlugin.TestLinker2.Navigation;

namespace ReSharperPlugin.TestLinker2.Actions
{
	[Action(
		Id,
		"Goto Linked Types (Test/Production)",
		IdeaShortcuts = new[] {"Shift+Control+I"},
		VsShortcuts = new[] {"Shift+Control+I"})]
	public class GotoLinkedTypes2Action
		: ContextNavigationActionBase<LinkedTypesNavigationProvider>
	{
		public const string Id = nameof(GotoLinkedTypes2Action);

		public override IActionRequirement GetRequirement(IDataContext dataContext)
		{
			return CurrentPsiFileRequirementNoCaches.FromDataContext(dataContext);
		}
	}
}
