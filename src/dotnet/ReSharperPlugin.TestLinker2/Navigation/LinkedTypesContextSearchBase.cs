using System.Collections.Generic;
using JetBrains.Application.DataContext;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.DocumentModel.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Navigation.ContextNavigation;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.TextControl.DataContext;

namespace ReSharperPlugin.TestLinker2.Navigation;

// TODO: Needed
public abstract class LinkedTypesContextSearchBase : DeclaredElementContextSearchBase<LinkedTypesSearchRequest>
{
	protected abstract LinkedTypesSearchRequest CreateSearchRequest(ITypeElement type, ITextControl textControl);

	protected override LinkedTypesSearchRequest CreateSearchRequest(
		IDataContext dataContext,
		IDeclaredElement element,
		IDeclaredElement initialTarget)
	{
		ITextControl textControl = dataContext.GetData(TextControlDataConstants.TEXT_CONTROL);
		if (textControl == null)
		{
			return null;
		}

		return element is ClassLikeTypeElement type ? CreateSearchRequest(type, textControl) : null;
	}

	protected override IEnumerable<IDeclaredElement> GetElementCandidates(
		IDataContext context,
		ReferencePreferenceKind kind,
		bool updateOnly)
	{
		ISolution solution = context.GetData(ProjectModelDataConstants.SOLUTION);
		if (solution == null)
			return base.GetElementCandidates(context, kind, updateOnly);

		DocumentOffset caretOffset = context.GetData(DocumentModelDataConstants.EDITOR_CONTEXT).NotNull().CaretOffset;

		IDeclaredElement typeOrMember = TextControlToPsi.GetContainingTypeOrTypeMember(solution, caretOffset);
		if (typeOrMember is not IClrDeclaredElement clr)
			return base.GetElementCandidates(context, kind, updateOnly);

		ITypeElement containingType = clr as ITypeElement ?? clr.GetContainingType();
		if (containingType == null)
			return base.GetElementCandidates(context, kind, updateOnly);

		return new[] {containingType};
	}
}