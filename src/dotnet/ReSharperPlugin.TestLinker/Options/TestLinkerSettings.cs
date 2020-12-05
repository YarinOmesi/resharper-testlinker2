using JetBrains.Application.Settings;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.TestLinker.Options
{
	[SettingsKey(typeof(UnitTestingSettings), "Settings for TestLinker")]
	public class TestLinkerSettings
	{
		[SettingsEntry(true, "Use Suffix Search")]
		public bool EnableSuffixSearch;

		[SettingsEntry(true, "Use Typeof Search")]
		public bool EnableTypeofSearch;

		[SettingsEntry(NamingStyle.Postfix, "Naming style for tests")]
		public NamingStyle NamingStyle;

		[SettingsEntry("Test,Spec,Tests,Specs", "Naming Suffixes")]
		public string NamingSuffixes;

		[SettingsEntry("SubjectAttribute", "Typeof Attribute")]
		public string TypeofAttributeName;
	}
}
