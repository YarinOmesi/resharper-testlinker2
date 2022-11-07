using System;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using ReSharperPlugin.TestLinker2.Options;

namespace ReSharperPlugin.TestLinker2.Utils;

public static class SettingsUtils
{
	public static TestLinkerSettings GetSettings(this ISolution solution)
	{
		ISettingsStore settingsStore = solution.GetComponent<ISettingsStore>();
		ContextRange contextRange = ContextRange.Smart(solution.ToDataContext());
		IContextBoundSettingsStore contextBoundSettingsStore = settingsStore.BindToContextTransient(contextRange);

		ISettingsOptimization settingsOptimization = solution.GetComponent<ISettingsOptimization>();
		return contextBoundSettingsStore.GetKey<TestLinkerSettings>(settingsOptimization);
	}

	public static string[] GetNamingSuffixesArray(this TestLinkerSettings settings)
	{
		return settings.NamingSuffixes.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
	}
}