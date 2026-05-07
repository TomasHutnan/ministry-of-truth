using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Engine;

public class DocumentGenerator(
    IRuleRepository ruleRepository,
    ITextRepository textRepository,
    IViolationRepository violationRepository,
    ComplexityCalculator complexityCalculator) : IDocumentGenerator
{
    private IRuleRepository _ruleRepository = ruleRepository;
    private ITextRepository _textRepository = textRepository;
    private IViolationRepository _violationRepository = violationRepository;

    private ComplexityCalculator _complexityCalculator = complexityCalculator;

    private Dictionary<string, Rule> _ruleById = [];
    private Dictionary<string, TextEntry> _textById = [];
    private Dictionary<(string, string), Violation> _violationByTextRuleIds = [];

    private Dictionary<int, List<TextEntry>> _textsByComplexity = [];

    private const int _minTextsPerDay = 3;
    private const int _textsAddedEveryTwoDays = 1;
    private const int _maxExtraTexts = 2;
    private const int _minComplexity = 1;
    private const int _maxComplexity = 5;
    private const int _targetComplexityRandomCount = 3;
    private const int _outOfRangePickCount = 1;
    private const int _ruleNamePreviewLength = 3;

    private static readonly Random _random = new();

    private bool _isInitialized = false;

    public async Task InitializeAsync()
    {
        _isInitialized = false;

        // IO
        var ruleTask = _ruleRepository.LoadAllAsync();
        var textTask = _textRepository.LoadAllAsync();
        var violationTask = _violationRepository.LoadAllAsync();

        await Task.WhenAll([ruleTask, textTask, violationTask]);

        // Compute
        Dictionary<string, Rule> ruleById = ruleTask.Result.ToDictionary((rule) => rule.Id);
        Dictionary<string, TextEntry> textById = textTask.Result.ToDictionary((text) => text.Id);

        Dictionary<(string, string), Violation> violationByTextRuleIds = new();
        foreach (Violation violation in violationTask.Result)
        {
            if (!textById.ContainsKey(violation.TextId))
            {
                throw new InvalidDataException($"Violations foreign key TextId with value {violation.TextId} not found in parent collection Texts.");
            }
            if (!ruleById.ContainsKey(violation.RuleId))
            {
                throw new InvalidDataException($"Violations foreign key RuleId with value {violation.RuleId} not found in parent collection Rules.");
            }

            violationByTextRuleIds[(violation.TextId, violation.RuleId)] = violation;
        }

        Dictionary<int, List<TextEntry>> textsByComplexity = new();
        foreach (var text in textById.Values)
        {
            int tier = _complexityCalculator.CalculateGroup(text.Content);

            if (!textsByComplexity.TryGetValue(tier, out var list))
            {
                list = new List<TextEntry>();
                textsByComplexity[tier] = list;
            }

            list.Add(text);
        }

        // Commit
        _ruleById = ruleById;
        _textById = textById;
        _violationByTextRuleIds = violationByTextRuleIds;
        _textsByComplexity = textsByComplexity;

        _isInitialized = true;
    }

    public DayPackage CreateDayPackage(int dayNumber, int targetComplexity)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Document generator is not initialized.");
        }

        int clampedTargetComplexity = Math.Clamp(targetComplexity, _minComplexity, _maxComplexity);
        int textCount = _minTextsPerDay + ((dayNumber - 1) / 2) * _textsAddedEveryTwoDays;
        textCount = Math.Min(textCount, _minTextsPerDay + _maxExtraTexts);

        List<TextEntry> chosenTexts = new(textCount);
        HashSet<string> violationIds = new();

        var availableTexts = _textById.Values.ToList();
        var targetTexts = _textsByComplexity.TryGetValue(clampedTargetComplexity, out var textsAtTarget)
            ? textsAtTarget
            : [];

        IEnumerable<TextEntry> primaryPool = targetTexts.Count > 0 ? targetTexts : availableTexts;
        int preferredCount = Math.Min(textCount, Math.Max(1, (int)Math.Ceiling(textCount * 0.7)));

        AddRandomUniqueTexts(chosenTexts, primaryPool, preferredCount);
        if (chosenTexts.Count < textCount)
        {
            var secondaryPool = targetTexts.Count > 0
                ? availableTexts.Where(text => !_textsByComplexity.TryGetValue(clampedTargetComplexity, out var list) || !list.Contains(text))
                : availableTexts;

            AddRandomUniqueTexts(chosenTexts, secondaryPool, textCount - chosenTexts.Count);
        }

        if (chosenTexts.Count < textCount)
        {
            AddRandomUniqueTexts(chosenTexts, availableTexts, textCount - chosenTexts.Count);
        }

        foreach (var text in chosenTexts)
        {
            foreach (var rule in _ruleById.Values)
            {
                if (_violationByTextRuleIds.ContainsKey((text.Id, rule.Id)))
                {
                    violationIds.Add(text.Id);
                    break;
                }
            }
        }

        return new DayPackage("RULE TODO", new Queue<TextEntry>(chosenTexts), violationIds);
    }

    private void AddRandomUniqueTexts(List<TextEntry> chosenTexts, IEnumerable<TextEntry> source, int count)
    {
        if (count <= 0)
        {
            return;
        }

        var candidates = source.Where(text => !chosenTexts.Any(chosen => chosen.Id == text.Id)).ToList();
        while (count > 0 && candidates.Count > 0)
        {
            int index = _random.Next(candidates.Count);
            chosenTexts.Add(candidates[index]);
            candidates.RemoveAt(index);
            count--;
        }
    }
}