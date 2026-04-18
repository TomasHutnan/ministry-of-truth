using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Engine;

public class GameEngine(
    IHighScoreStore highScoreStore,
    IRuleRepository ruleRepository,
    ITextRepository textRepository,
    IViolationRepository violationRepository,
    ComplexityCalculator complexityCalculator)
{
    private IHighScoreStore _highScoreStore = highScoreStore;
    private IRuleRepository _ruleRepository = ruleRepository;
    private ITextRepository _textRepository = textRepository;
    private IViolationRepository _violationRepository = violationRepository;

    private ComplexityCalculator _complexityCalculator = complexityCalculator;

    private int _highScore = 0;
    private Dictionary<string, Rule> _ruleById = [];
    private Dictionary<string, TextEntry> _textById = [];
    private Dictionary<(string, string), Violation> _violationByTextRuleIds = [];

    private Dictionary<int, List<TextEntry>> _textsByComplexity = [];

    public async Task InitializeAsync()
    {
        // IO
        var ruleTask = _ruleRepository.LoadAllAsync();
        var textTask = _textRepository.LoadAllAsync();
        var violationTask = _violationRepository.LoadAllAsync();
        var highScoreTask = _highScoreStore.LoadAsync();

        await Task.WhenAll([ruleTask, textTask, violationTask, highScoreTask]);

        // Compute
        int highscore = highScoreTask.Result;
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
        _highScore = highscore;
        _ruleById = ruleById;
        _textById = textById;
        _violationByTextRuleIds = violationByTextRuleIds;
        _textsByComplexity = textsByComplexity;
    }
}
