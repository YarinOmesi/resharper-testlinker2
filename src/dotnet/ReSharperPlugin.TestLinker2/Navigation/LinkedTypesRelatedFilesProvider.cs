using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation;
using JetBrains.ReSharper.Psi;
using ReSharperPlugin.TestLinker2.Utils;

namespace ReSharperPlugin.TestLinker2.Navigation;

[RelatedFilesProvider(typeof(KnownProjectFileType))]
public class LinkedTypesRelatedFilesProvider : IRelatedFilesProvider
{
	public IEnumerable<RelatedFileOccurence> GetRelatedFiles(IProjectFile projectFile)
	{
		var sourceFile = projectFile.ToSourceFile();
		if (sourceFile == null)
			yield break;

		var services = sourceFile.GetPsiServices();
		var sourceTypes = services.Symbols.GetTypesAndNamespacesInFile(sourceFile).OfType<ITypeElement>();
		var linkedTypes = sourceTypes.SelectMany(LinkedTypesUtil.GetLinkedTypes);
		foreach (var linkedType in linkedTypes)
		{
			var linkedFile = linkedType.GetSingleOrDefaultSourceFile().ToProjectFile();
			yield return new RelatedFileOccurence(linkedFile, "Linked", projectFile);
		}
	}
}