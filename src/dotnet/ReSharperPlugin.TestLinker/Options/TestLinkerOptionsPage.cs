using System;
using System.Linq.Expressions;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Options;
using JetBrains.Application.UI.Options.OptionsDialog;
using JetBrains.DataFlow;
using JetBrains.IDE.UI.Extensions;
using JetBrains.IDE.UI.Options;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperPlugin.TestLinker.Options
{
	[OptionsPage(Id, PageTitle, typeof(TestLinkerThemedIcons.TestLinker),
		ParentId = UnitTestingPages.General,
		NestingType = OptionPageNestingType.Inline,
		IsAlignedWithParent = true,
		Sequence = 0.1d)]
	public class TestLinkerOptionsPage : BeSimpleOptionsPage
	{
		private new const string Id = nameof(TestLinkerOptionsPage);
		private const string PageTitle = "Test Linker";

		private readonly Lifetime _lifetime;

		public TestLinkerOptionsPage(
			Lifetime lifetime,
			OptionsPageContext optionsPageContext,
			OptionsSettingsSmartContext optionsSettingsSmartContext)
			: base(lifetime, optionsPageContext, optionsSettingsSmartContext)
		{
			_lifetime = lifetime;

			AddHeader("Navigation");

			AddTextBox((TestLinkerSettings x) => x.NamingSuffixes, "Name suffixes for tests (comma-separated):");
			AddTextBox((TestLinkerSettings x) => x.TypeofAttributeName, "Attribute name for typeof mentions:");
		}

		private void AddTextBox<TKeyClass>(Expression<Func<TKeyClass, string>> lambdaExpression, string description)
		{
			var property = new Property<string>(description);
			OptionsSettingsSmartContext.SetBinding(_lifetime, lambdaExpression, property);
			var control = property.GetBeTextBox(_lifetime);
			AddControl(control.WithDescription(description, _lifetime));
		}
	}
}
