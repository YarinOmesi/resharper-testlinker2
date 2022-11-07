using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using JetBrains.Util.Logging;

namespace ReSharperPlugin.TestLinker2.Navigation;

// TODO: Needed
[ShellFeaturePart]
public class LinkedTypesContextSearch : LinkedTypesContextSearchBase
{
	private readonly ILogger _logger;

	public LinkedTypesContextSearch()
	{
		_logger = Logger.GetLogger<LinkedTypesContextSearch>();
	}

	protected override LinkedTypesSearchRequest CreateSearchRequest(ITypeElement type, ITextControl textControl)
	{
		_logger.Info($"CreateSearchRequest With Type: {type}");
		return new LinkedTypesSearchRequest(type, textControl, Logger.GetLogger<LinkedTypesContextSearch>());
	}
}