using JetBrains.ReSharper.IntentionsTests.Navigation;
using NUnit.Framework;

namespace ReSharperPlugin.TestLinker2.Tests
{
	public class MultipleTestSingleProdTests : AllNavigationProvidersTestBase
	{
		protected override string ExtraPath => "Navigation";

		protected override string RelativeTestDataPath => "MultipleTestSingleProd";

		[Test]
		public void MultipleTestSingleProd()
		{
			DoNamedTest("MultipleTestSingleProdTests1.cs", "MultipleTestSingleProdTests2.cs");
		}
	}
}