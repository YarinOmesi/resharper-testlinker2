using JetBrains.Application.UI.Controls.JetPopupMenu;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.RichText;

namespace ReSharperPlugin.TestLinker2.Navigation
{
	[OccurrencePresenter]
	public class LinkedTypesOccurrencePresenter : DeclaredElementOccurrencePresenter
	{
		public override bool IsApplicable(IOccurrence occurrence)
		{
			return occurrence is LinkedTypesOccurrence;
		}

		protected override void DisplayMainText(IMenuItemDescriptor descriptor, IOccurrence occurrence,
			OccurrencePresentationOptions options,
			IDeclaredElement declaredElement)
		{
			base.DisplayMainText(descriptor, occurrence, options, declaredElement);
			if (occurrence is LinkedTypesOccurrence { HasNameDerived: true })
				descriptor.Text.SetStyle(JetFontStyles.Bold);
		}
	}
}