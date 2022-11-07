using JetBrains.ReSharper.Psi;

namespace ReSharperPlugin.TestLinker2.Utils;

public static class TypeElementExtensions
{
	public static bool IsNamesDerived(this ITypeElement element, ITypeElement other)
	{
		return element.ShortName.Contains(other.ShortName) ||
		       other.ShortName.Contains(element.ShortName);
	}

}