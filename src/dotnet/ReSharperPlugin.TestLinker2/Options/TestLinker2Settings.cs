using JetBrains.Application.Settings;
using JetBrains.ReSharper.UnitTestFramework.Settings;

namespace ReSharperPlugin.TestLinker2.Options;

[SettingsKey(typeof(UnitTestingSettings), "Settings for Test Linker 2")]
public class TestLinkerSettings
{
	[SettingsEntry("Test,Spec,Tests,Specs", "Naming Suffixes")]
	public string NamingSuffixes;

	[SettingsEntry("SubjectAttribute", "Typeof Attribute")]
	public string TypeofAttributeName;
}