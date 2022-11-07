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
		IPsiSourceFile sourceFile = projectFile.ToSourceFile();
		if (sourceFile == null)
			yield break;

		IPsiServices services = sourceFile.GetPsiServices();
		IEnumerable<ITypeElement> sourceTypes = services.Symbols
			.GetTypesAndNamespacesInFile(sourceFile)
			.OfType<ITypeElement>();

		IEnumerable<ITypeElement> linkedTypes = sourceTypes.SelectMany(LinkedTypesUtil.GetLinkedTypes);
		foreach (ITypeElement linkedType in linkedTypes)
		{
			IProjectFile linkedFile = linkedType.GetSingleOrDefaultSourceFile().ToProjectFile();
			yield return new RelatedFileOccurence(linkedFile, "Linked", projectFile);
		}
	}
}