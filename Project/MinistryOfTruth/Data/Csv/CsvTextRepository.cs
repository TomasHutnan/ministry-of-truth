using CsvHelper;
using CsvHelper.Configuration;
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
        if (!RepositoryExists())
        {
            return Task.FromResult<IReadOnlyCollection<TextEntry>>(Array.Empty<TextEntry>());
        }

        using var stream = File.OpenRead(_textsCsvPath);
        using var reader = new StreamReader(stream);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            Quote = '"',
            Escape = '"',
            BadDataFound = null,
            TrimOptions = TrimOptions.None
        };

        using var csv = new CsvReader(reader, config);

        List<TextEntry> texts = new List<TextEntry>();

        foreach (CsvTextRow text in csv.GetRecords<CsvTextRow>())
        {
            texts.Add(new TextEntry(text.Id, text.Content));
        }

        return Task.FromResult<IReadOnlyCollection<TextEntry>>(texts);
    }

    public async Task SetAllAsync(IEnumerable<TextEntry> texts)
    {
        ArgumentNullException.ThrowIfNull(texts);

        string directory = Path.GetDirectoryName(_textsCsvPath)!;
        Directory.CreateDirectory(directory);

        string tempPath = Path.Combine(directory, $"{Path.GetRandomFileName()}.tmp");

        try
        {
            await using (var stream = File.Create(tempPath))
            await using (var writer = new StreamWriter(stream))
            await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                await csv.WriteRecordsAsync(texts.Select(text => new CsvTextRow
                {
                    Id = text.Id,
                    Content = text.Content
                }));
            }

            if (File.Exists(_textsCsvPath))
            {
                File.Replace(tempPath, _textsCsvPath, null);
            }
            else
            {
                File.Move(tempPath, _textsCsvPath);
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
        return File.Exists(_textsCsvPath);
    }
}
