using System;
using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;
using ReSharperPlugin.TestLinker2.Navigation;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.TestLinker2.Tests
{
	[ZoneDefinition]
	public interface ITestLinker2TestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
	{
	}

	[ZoneMarker]
	public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>,
		IRequire<ITestLinker2TestEnvironmentZone>
	{
	}

	[SetUpFixture]
	public class TestLinker2TestsAssembly : ExtensionTestEnvironmentAssembly<ITestLinker2TestEnvironmentZone>
	{
		public TestLinker2TestsAssembly()
		{
			// an explicit reference to a type from the production code is necessary,
			// because otherwise the JetBrains testing framework won't load the Rider/R# plugin.
			Type type = typeof(LinkedTypesNavigationProvider);
		}
	}
}