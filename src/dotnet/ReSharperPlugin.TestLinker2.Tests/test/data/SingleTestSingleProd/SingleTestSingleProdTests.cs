using BusinessLogic;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class SingleTestSingleProdTests
{
	[Test]
	public void Add()
	{
		var testee = new SingleTestSingleProd();

		var result = testee.Add(1, 2);

		Assert.AreEqual(3, result);
	}
}