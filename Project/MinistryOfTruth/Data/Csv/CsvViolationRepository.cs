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

    public async Task SetAllAsync(IEnumerable<Violation> violations)
    {
        ArgumentNullException.ThrowIfNull(violations);

        string directory = Path.GetDirectoryName(_violationsCsvPath)!;
        Directory.CreateDirectory(directory);

        string tempPath = Path.Combine(directory, $"{Path.GetRandomFileName()}.tmp");

        try
        {
            await using (var stream = File.Create(tempPath))
            await using (var writer = new StreamWriter(stream))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(violations.Select(violation => new CsvViolationRow
                {
                    TextId = violation.TextId,
                    RuleId = violation.RuleId,
                    Justification = violation.Justification
                }));
            }

            if (File.Exists(_violationsCsvPath))
            {
                File.Replace(tempPath, _violationsCsvPath, null);
            }
            else
            {
                File.Move(tempPath, _violationsCsvPath);
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
}
