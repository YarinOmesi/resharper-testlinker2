using BusinessLogic;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class MultipleTestSingleProdTests2
{
	[Test]
	public void Add()
	{
		var testee = new MultipleTestSingleProd();

		var result = testee.Add(1, 2);

		Assert.AreEqual(3, result);
	}
}