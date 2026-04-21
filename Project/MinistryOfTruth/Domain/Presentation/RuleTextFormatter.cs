using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Presentation;

public class RuleTextFormatter
{
    public string BuildRuleText(Rule rule)
    {
        // TODO: Add more variants - maybe introduce levels of agrresivity
        return $"Any mention of {rule.Keyword} is prohibited.";
    }
}
