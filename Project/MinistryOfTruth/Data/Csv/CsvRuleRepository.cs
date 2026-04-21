using CsvHelper;
using MinistryOfTruth.Data.Parsing;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using System.Globalization;

namespace MinistryOfTruth.Data.Csv;

public class CsvRuleRepository : IRuleRepository
{
    private static readonly string _rulesCsvPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "rules.csv");

    public Task<IReadOnlyCollection<Rule>> LoadAllAsync()
    {
        if (!File.Exists(_rulesCsvPath))
        {
            return Task.FromResult<IReadOnlyCollection<Rule>>(Array.Empty<Rule>());
        }

        using var stream = File.OpenRead(_rulesCsvPath);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        List<Rule> rules = new List<Rule>();

        foreach (CsvRuleRow rule in csv.GetRecords<CsvRuleRow>())
        {
            rules.Add(new Rule(rule.Id, rule.Keyword, rule.Number.ToLower() == "plural"));
        }

        return Task.FromResult<IReadOnlyCollection<Rule>>(rules);
    }
}
