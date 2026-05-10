using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Engine;

public class DocumentGenerator(
    IRuleRepository ruleRepository,
    ITextRepository textRepository,
    IViolationRepository violationRepository,
    ComplexityCalculator complexityCalculator) : IDocumentGenerator
{
    private readonly IRuleRepository _ruleRepository = ruleRepository;
    private readonly ITextRepository _textRepository = textRepository;
    private readonly IViolationRepository _violationRepository = violationRepository;

    private readonly ComplexityCalculator _complexityCalculator = complexityCalculator;

    private Dictionary<string, Rule> _ruleById = [];
    private Dictionary<string, TextEntry> _textById = [];
    private Dictionary<(string, string), Violation> _violationByTextRuleIds = [];

    private Dictionary<int, List<TextEntry>> _textsByComplexity = [];

    private const int _minTextsPerDay = 3;
    private const int _textsAddedEveryTwoDays = 1;
    private const int _maxExtraTexts = 2;
    private const int _minComplexity = 1;
    private const int _maxComplexity = 5;

    private static readonly Random _random = new();

    private bool _isInitialized = false;
    private readonly Lock _stateLocker = new();

    public async Task InitializeAsync()
    {
        var ruleTask = _ruleRepository.LoadAllAsync();
        var textTask = _textRepository.LoadAllAsync();
        var violationTask = _violationRepository.LoadAllAsync();

        await Task.WhenAll(ruleTask, textTask, violationTask);

        var ruleById = ruleTask.Result.ToDictionary(r => r.Id);
        var textById = textTask.Result.ToDictionary(t => t.Id);

        var violationByTextRuleIds = new Dictionary<(string, string), Violation>();
        foreach (var violation in violationTask.Result)
        {
            if (!textById.ContainsKey(violation.TextId))
                throw new InvalidDataException($"Violations foreign key TextId with value {violation.TextId} not found in parent collection Texts.");
            if (!ruleById.ContainsKey(violation.RuleId))
                throw new InvalidDataException($"Violations foreign key RuleId with value {violation.RuleId} not found in parent collection Rules.");

            violationByTextRuleIds[(violation.TextId, violation.RuleId)] = violation;
        }

        var textsByComplexity = new Dictionary<int, List<TextEntry>>();
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

        lock (_stateLocker)
        {
            _ruleById = ruleById;
            _textById = textById;
            _violationByTextRuleIds = violationByTextRuleIds;
            _textsByComplexity = textsByComplexity;
            _isInitialized = true;
        }
    }

    public DayPackage CreateDayPackage(int dayNumber, int targetComplexity)
    {
        lock (_stateLocker)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Document generator is not initialized. Call InitializeAsync first.");
            }

            var rules = _ruleById.Values.ToList();
            if (rules.Count == 0)
            {
                throw new InvalidOperationException("No rules available.");
            }

            // Build mapping rule -> violating texts
            var ruleToViolatingTexts = new Dictionary<string, List<TextEntry>>();
            foreach (var kvp in _violationByTextRuleIds)
            {
                var (textId, ruleId) = kvp.Key;
                if (!_textById.TryGetValue(textId, out var text))
                    continue;
                if (!ruleToViolatingTexts.TryGetValue(ruleId, out var list))
                {
                    list = new List<TextEntry>();
                    ruleToViolatingTexts[ruleId] = list;
                }
                list.Add(text);
            }

            // Weighted pick of a rule by number of violating texts (fallback to uniform)
            int totalViolations = ruleToViolatingTexts.Values.Sum(l => l.Count);
            string selectedRuleId;
            if (totalViolations == 0)
            {
                selectedRuleId = rules[_random.Next(rules.Count)].Id;
            }
            else
            {
                int pick = _random.Next(totalViolations);
                int acc = 0;
                selectedRuleId = rules[0].Id;
                foreach (var r in rules)
                {
                    int count = ruleToViolatingTexts.TryGetValue(r.Id, out var list) ? list.Count : 0;
                    acc += count;
                    if (pick < acc)
                    {
                        selectedRuleId = r.Id;
                        break;
                    }
                }
            }

            var selectedRule = _ruleById[selectedRuleId];

            int clampedTargetComplexity = Math.Clamp(targetComplexity, _minComplexity, _maxComplexity);
            int textCount = _minTextsPerDay + ((dayNumber - 1) / 2) * _textsAddedEveryTwoDays;
            textCount = Math.Min(textCount, _minTextsPerDay + _maxExtraTexts);

            int desiredViolations = textCount / 2; // aim for half

            var allTexts = _textById.Values.ToList();
            var targetTexts = _textsByComplexity.TryGetValue(clampedTargetComplexity, out var textsAtTarget) ? textsAtTarget : new List<TextEntry>();

            var violatingPool = ruleToViolatingTexts.TryGetValue(selectedRuleId, out var vlist) ? vlist : new List<TextEntry>();
            var nonViolatingPool = allTexts.Where(t => !violatingPool.Any(v => v.Id == t.Id)).ToList();

            var chosen = new List<TextEntry>();

            // 1) take violations from target complexity first
            var violatingTarget = violatingPool.Where(t => targetTexts.Contains(t)).ToList();
            AddRandomUniqueTexts(chosen, violatingTarget, Math.Min(desiredViolations, violatingTarget.Count));

            // 2) take other violations if needed
            AddRandomUniqueTexts(chosen, violatingPool.Except(chosen), Math.Min(desiredViolations - chosen.Count, violatingPool.Except(chosen).Count()));

            // 3) fill remaining slots preferring target complexity non-violations
            int remaining = textCount - chosen.Count;
            var targetNonViolations = targetTexts.Where(t => !chosen.Any(c => c.Id == t.Id) && !violatingPool.Any(v => v.Id == t.Id)).ToList();
            AddRandomUniqueTexts(chosen, targetNonViolations, Math.Min(remaining, targetNonViolations.Count));

            // 4) fill from non-violating pool
            remaining = textCount - chosen.Count;
            AddRandomUniqueTexts(chosen, nonViolatingPool.Except(chosen), remaining);

            // 5) final fallback: any remaining texts
            remaining = textCount - chosen.Count;
            if (remaining > 0)
            {
                AddRandomUniqueTexts(chosen, allTexts.Except(chosen), remaining);
            }

            // compute violation ids for the selected rule
            var violationIds = new HashSet<string>(chosen.Where(t => _violationByTextRuleIds.ContainsKey((t.Id, selectedRuleId))).Select(t => t.Id));

            // try swapping in more violations if we didn't reach desiredViolations
            int currentViolations = violationIds.Count;
            if (currentViolations < desiredViolations && violatingPool.Count > currentViolations)
            {
                var nonViolatingChosen = chosen.Where(t => !violationIds.Contains(t.Id)).ToList();
                var remainingViolations = violatingPool.Where(t => !chosen.Any(c => c.Id == t.Id)).ToList();

                int need = desiredViolations - currentViolations;
                for (int i = 0; i < need && remainingViolations.Count > 0 && nonViolatingChosen.Count > 0; i++)
                {
                    int replaceIndex = _random.Next(nonViolatingChosen.Count);
                    var replaceText = nonViolatingChosen[replaceIndex];
                    nonViolatingChosen.RemoveAt(replaceIndex);

                    int pickIndex = _random.Next(remainingViolations.Count);
                    var newViolation = remainingViolations[pickIndex];
                    remainingViolations.RemoveAt(pickIndex);

                    int idx = chosen.FindIndex(t => t.Id == replaceText.Id);
                    if (idx >= 0)
                    {
                        chosen[idx] = newViolation;
                    }
                }

                violationIds = [.. chosen.Where(t => _violationByTextRuleIds.ContainsKey((t.Id, selectedRuleId))).Select(t => t.Id)];
            }

            return new DayPackage(selectedRule, new Queue<TextEntry>(chosen), violationIds);
        }
    }

    private static void AddRandomUniqueTexts(List<TextEntry> chosenTexts, IEnumerable<TextEntry> source, int count)
    {
        if (count <= 0) return;

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