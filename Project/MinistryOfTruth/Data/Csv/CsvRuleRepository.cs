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
        if (!RepositoryExists())
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

    public async Task SetAllAsync(IEnumerable<Rule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        string directory = Path.GetDirectoryName(_rulesCsvPath)!;
        Directory.CreateDirectory(directory);

        string tempPath = Path.Combine(directory, $"{Path.GetRandomFileName()}.tmp");

        try
        {
            await using (var stream = File.Create(tempPath))
            await using (var writer = new StreamWriter(stream))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(rules.Select(rule => new CsvRuleRow
                {
                    Id = rule.Id,
                    Keyword = rule.Keyword,
                    Number = rule.IsPlural ? "plural" : "singular"
                }));
            }

            if (File.Exists(_rulesCsvPath))
            {
                File.Replace(tempPath, _rulesCsvPath, null);
            }
            else
            {
                File.Move(tempPath, _rulesCsvPath);
            }
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    public bool RepositoryExists()
    {
        return File.Exists(_rulesCsvPath);
    }
}
