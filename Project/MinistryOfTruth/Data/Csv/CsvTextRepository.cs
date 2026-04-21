using CsvHelper;
using MinistryOfTruth.Data.Parsing;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using System.Globalization;

namespace MinistryOfTruth.Data.Csv;

public class CsvTextRepository : ITextRepository
{
    private static readonly string _textsCsvPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "texts.csv");

    public Task<IReadOnlyCollection<TextEntry>> LoadAllAsync()
    {
        if (!File.Exists(_textsCsvPath))
        {
            return Task.FromResult<IReadOnlyCollection<TextEntry>>(Array.Empty<TextEntry>());
        }

        using var stream = File.OpenRead(_textsCsvPath);
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        List<TextEntry> texts = new List<TextEntry>();

        foreach (CsvTextRow text in csv.GetRecords<CsvTextRow>())
        {
            texts.Add(new TextEntry(text.Id, text.Content));
        }

        return Task.FromResult<IReadOnlyCollection<TextEntry>>(texts);
    }
}
