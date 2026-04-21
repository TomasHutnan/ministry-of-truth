using CsvHelper;
using MinistryOfTruth.Data.Parsing;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using System.Globalization;

namespace MinistryOfTruth.Data.Csv;

public class CsvViolationRepository : IViolationRepository
{
    private static readonly string _violationsCsvPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "violations.csv");

    public Task<IReadOnlyCollection<Violation>> LoadAllAsync()
    {
        if (!File.Exists(_violationsCsvPath))
        {
            return Task.FromResult<IReadOnlyCollection<Violation>>(Array.Empty<Violation>());
        }

        using var stream = File.OpenRead(_violationsCsvPath);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        List<Violation> violations = new List<Violation>();

        foreach (CsvViolationRow violation in csv.GetRecords<CsvViolationRow>())
        {
            violations.Add(new Violation(violation.TextId, violation.RuleId, violation.Justification));
        }

        return Task.FromResult<IReadOnlyCollection<Violation>>(violations);
    }
}
