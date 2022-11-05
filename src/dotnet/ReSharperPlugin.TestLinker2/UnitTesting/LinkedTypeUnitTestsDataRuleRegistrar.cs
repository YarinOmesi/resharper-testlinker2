using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework.Actions;
using JetBrains.ReSharper.UnitTestFramework.Criteria;
using JetBrains.ReSharper.UnitTestFramework.Execution.Launch;
using JetBrains.ReSharper.UnitTestFramework.Features;
using JetBrains.Util;
using ReSharperPlugin.TestLinker2.Utils;

namespace ReSharperPlugin.TestLinker2.UnitTesting;

[SolutionComponent]
public class LinkedTypeUnitTestsDataRuleRegistrar
{
	private readonly IUnitTestPsiManager _unitTestElementStuff;

	public LinkedTypeUnitTestsDataRuleRegistrar(
		Lifetime lifetime,
		DataContexts dataContexts,
		IUnitTestPsiManager unitTestElementStuff)
	{
		_unitTestElementStuff = unitTestElementStuff;

		var dataRule = new DataRule<UnitTestElements>.DesperateDataRule(
			"ProjectModelToUnitTestElements",
			new UnitTestDataConstants.ElementsDataConstants("UnitTestElements").Selected,
			LinkedTypeUnitTestsDataRule);

		dataContexts.RegisterDataRule(lifetime, dataRule);
	}

	private UnitTestElements LinkedTypeUnitTestsDataRule(IDataContext context)
	{
		var linkedTypes = LinkedTypesUtil.GetLinkedTypes(context);
		var relevantTests = linkedTypes.Select(x => _unitTestElementStuff.GetElement(x)).WhereNotNull();
		return new UnitTestElements(new TestAncestorCriterion(relevantTests));
	}
}