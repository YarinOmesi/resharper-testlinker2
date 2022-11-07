using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Refactorings.Rename;
using ReSharperPlugin.TestLinker2.Utils;

namespace ReSharperPlugin.TestLinker2.Refactorings;

[DerivedRenamesEvaluator]
public class LinkedTypesDerivedNameEvaluator : IDerivedRenamesEvaluator
{
	public bool SuggestedElementsHaveDerivedName => true;

	public IEnumerable<IDeclaredElement> CreateFromElement(
		[NotNull] IEnumerable<IDeclaredElement> initialElement,
		[NotNull] DerivedElement derivedElement)
	{
		return GetRelatedTypesWithDerivedName(derivedElement.DeclaredElement);
	}

	public IEnumerable<IDeclaredElement> CreateFromReference([NotNull] IReference reference,
		[NotNull] IDeclaredElement declaredElement)
	{
		return GetRelatedTypesWithDerivedName(declaredElement);
	}

	private static IEnumerable<IDeclaredElement> GetRelatedTypesWithDerivedName(IDeclaredElement declaredElement)
	{
		if (declaredElement is not ITypeElement typeElement)
			return Enumerable.Empty<IDeclaredElement>();

		// TODO get linked types by name
		IReadOnlyCollection<ITypeElement> linkedTypes = LinkedTypesUtil.GetLinkedTypes(typeElement);
		return linkedTypes.Where(x =>
			typeElement.ShortName.Contains(x.ShortName) || x.ShortName.Contains(typeElement.ShortName));
	}
}