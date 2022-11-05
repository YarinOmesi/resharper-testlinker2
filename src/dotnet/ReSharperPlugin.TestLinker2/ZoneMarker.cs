using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TextControl;

namespace ReSharperPlugin.TestLinker2;

[ZoneMarker]
public class ZoneMarker
	: IRequire<IUnitTestingZone>,
		IRequire<ILanguageCSharpZone>,
		IRequire<DaemonEngineZone>,
		IRequire<ITextControlsZone>
{
}