using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Navigation.Requests;
using JetBrains.ReSharper.Feature.Services.Occurrences;
using JetBrains.ReSharper.Psi;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharperPlugin.TestLinker2.Utils;

namespace ReSharperPlugin.TestLinker2.Navigation;

// TODO: this seems to be the core
public sealed class LinkedTypesSearchRequest : SearchRequest
{
	private readonly ITextControl _textControl;
	private readonly ITypeElement _typeElement;
	private readonly ILogger _logger;

	public LinkedTypesSearchRequest(ITypeElement typeElement, ITextControl textControl, ILogger logger)
	{
		_typeElement = typeElement;
		_textControl = textControl;
		_logger = logger;
	}

	public override string Title => $"Linked Types for {_typeElement.ShortName}";

	public override ISolution Solution => _typeElement.GetSolution();

	public override ICollection SearchTargets => new IDeclaredElementEnvoy[] {
		new DeclaredElementEnvoy<IDeclaredElement>(_typeElement)
	};

	public override ICollection<IOccurrence> Search(IProgressIndicator progressIndicator)
	{
		bool isTypeElementValid = _typeElement.IsValid();
		_logger.Info($"Searching For TypeElement: {_typeElement} isValid:{isTypeElementValid}");
		if (!isTypeElementValid)
		{
			return EmptyList<IOccurrence>.InstanceList;
		}

		IReadOnlyCollection<ITypeElement> linkedTypes = LinkedTypesUtil.GetLinkedTypes(_typeElement);
		_logger.Info($"Found {linkedTypes.Count} LinkedTypes, [{string.Join(", ", linkedTypes)}]");
		if (linkedTypes.Count == 0)
		{
			_logger.Info($"TryCreateTestOrProductionClass");
			ModificationUtility.TryCreateTestOrProductionClass(_typeElement, _textControl);
		}

		return linkedTypes
			.Select(x =>
				new LinkedTypesOccurrence(x, OccurrenceType.Occurrence, _typeElement.IsNamesDerived(x)))
			.ToArray();
	}
}