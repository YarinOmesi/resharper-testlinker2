using JetBrains.ReSharper.IntentionsTests.Navigation;
using NUnit.Framework;

namespace ReSharperPlugin.TestLinker2.Tests
{
	public class SingleTestSingleProdTests : AllNavigationProvidersTestBase
	{
		protected override string ExtraPath => "Navigation";

		protected override string RelativeTestDataPath => "SingleTestSingleProd";

		[Test]
		public void SingleTestSingleProd()
		{
			DoNamedTest("SingleTestSingleProdTests.cs");
		}
	}
}