using JetBrains.Annotations;
using JetBrains.Application.UI.PopupLayout;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.NavigationExtensions;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace ReSharperPlugin.TestLinker2.Navigation;

public class LinkedTypesOccurrence : DeclaredElementOccurrence
{
	public bool HasNameDerived { get; }

	public LinkedTypesOccurrence(
		[NotNull] IDeclaredElement element,
		OccurrenceType occurrenceKind,
		bool hasNameDerived
	) : base(element, occurrenceKind)
	{
		HasNameDerived = hasNameDerived;
	}

	public override bool Navigate(
		ISolution solution,
		PopupWindowContextSource windowContext,
		bool transferFocus,
		TabOptions tabOptions = TabOptions.Default
	)
	{
		ITextControlManager textControlManager = solution.GetComponent<ITextControlManager>();

		IDeclaredElement declaredElement = OccurrenceElement.NotNull().GetValidDeclaredElement();
		if (declaredElement == null)
			return false;

		foreach (IDeclaration declaration in declaredElement.GetDeclarations())
		{
			IPsiSourceFile sourceFile = declaration.GetSourceFile();
			if (sourceFile == null)
				continue;

			foreach (ITextControl textControl in textControlManager.TextControls)
			{
				if (textControl.Document != sourceFile.Document)
					continue;

				DocumentRange declarationRange = declaration.GetDocumentRange();
				DocumentOffset textControlOffset = textControl.Caret.DocumentOffset();
				if (!declarationRange.Contains(textControlOffset))
					continue;

				PopupWindowContextSource popupWindowContextSource = solution.GetComponent<IMainWindowPopupWindowContext>().Source;
				return sourceFile.Navigate(
					textControl.Selection.OneDocRangeWithCaret(),
					transferFocus,
					tabOptions,
					popupWindowContextSource);
			}
		}

		return base.Navigate(solution, windowContext, transferFocus, tabOptions);
	}
}