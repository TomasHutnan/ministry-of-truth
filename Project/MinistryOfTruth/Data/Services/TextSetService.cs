using System.IO.Compression;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Data.Parsing;
using CsvHelper;
using System.Globalization;

namespace MinistryOfTruth.Data.Services;

public class TextSetService : ITextSetLoader
{
    private readonly ITextRepository _textRepository;
    private readonly IRuleRepository _ruleRepository;
    private readonly IViolationRepository _violationRepository;
    private readonly IAppAssetProvider _assetProvider;

    private static readonly string _storageDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

    private const string DefaultTextSetAssetPath = "default_text_set.zip";

    public TextSetService(
        ITextRepository textRepository,
        IRuleRepository ruleRepository,
        IViolationRepository violationRepository,
        IAppAssetProvider assetProvider)
    {
        _textRepository = textRepository;
        _ruleRepository = ruleRepository;
        _violationRepository = violationRepository;
        _assetProvider = assetProvider;
    }

    public async Task LoadFromFileAsync(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Text set file not found: {filePath}");
        }

        await ExtractAndLoadTextSetAsync(filePath);
    }

    public async Task LoadDefaultAsync()
    {
        try
        {
            var assetStream = await _assetProvider.OpenAssetAsync(DefaultTextSetAssetPath);
            using (assetStream)
            {
                string tempZipPath = Path.Combine(_storageDirectory, $"default_{Path.GetRandomFileName()}.zip");
                try
                {
                    await using (var fileStream = File.Create(tempZipPath))
                    {
                        await assetStream.CopyToAsync(fileStream);
                    }

                    await ExtractAndLoadTextSetAsync(tempZipPath);
                }
                finally
                {
                    if (File.Exists(tempZipPath))
                    {
                        File.Delete(tempZipPath);
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            throw new InvalidOperationException(
                $"Default text set asset not found: {DefaultTextSetAssetPath}. " +
                "Please ensure the default_text_set.zip file is included in the app assets.", null);
        }
    }

    public async Task ResetToDefaultAsync()
    {
        await LoadDefaultAsync();
    }

    private async Task ExtractAndLoadTextSetAsync(string zipFilePath)
    {
        string tempExtractDir = Path.Combine(_storageDirectory, $"textset_{Path.GetRandomFileName()}");

        try
        {
            Directory.CreateDirectory(tempExtractDir);
            ZipFile.ExtractToDirectory(zipFilePath, tempExtractDir, overwriteFiles: true);

            string textsPath = Path.Combine(tempExtractDir, "texts.csv");
            string rulesPath = Path.Combine(tempExtractDir, "rules.csv");
            string violationsPath = Path.Combine(tempExtractDir, "violations.csv");

            if (!File.Exists(textsPath))
            {
                throw new InvalidOperationException("texts.csv not found in the text set archive.");
            }

            await LoadCsvFilesToRepositoriesAsync(textsPath, rulesPath, violationsPath);
        }
        finally
        {
            if (Directory.Exists(tempExtractDir))
            {
                Directory.Delete(tempExtractDir, recursive: true);
            }
        }
    }

    private async Task LoadCsvFilesToRepositoriesAsync(string textsPath, string rulesPath, string violationsPath)
    {
        var texts = await LoadTextsFromCsvAsync(textsPath);
        await _textRepository.SetAllAsync(texts);

        if (File.Exists(rulesPath))
        {
            var rules = await LoadRulesFromCsvAsync(rulesPath);
            await _ruleRepository.SetAllAsync(rules);
        }

        if (File.Exists(violationsPath))
        {
            var violations = await LoadViolationsFromCsvAsync(violationsPath);
            await _violationRepository.SetAllAsync(violations);
        }
    }

    private async Task<IEnumerable<TextEntry>> LoadTextsFromCsvAsync(string path)
    {
        return await Task.Run(() =>
        {
            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var texts = new List<TextEntry>();
            foreach (var record in csv.GetRecords<CsvTextRow>())
            {
                texts.Add(new TextEntry(record.Id, record.Content));
            }
            return texts;
        });
    }

    private async Task<IEnumerable<Rule>> LoadRulesFromCsvAsync(string path)
    {
        return await Task.Run(() =>
        {
            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rules = new List<Rule>();
            foreach (var record in csv.GetRecords<CsvRuleRow>())
            {
                rules.Add(new Rule(record.Id, record.Keyword, record.Number.ToLower() == "plural"));
            }
            return rules;
        });
    }

    private async Task<IEnumerable<Violation>> LoadViolationsFromCsvAsync(string path)
    {
        return await Task.Run(() =>
        {
            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var violations = new List<Violation>();
            foreach (var record in csv.GetRecords<CsvViolationRow>())
            {
                violations.Add(new Violation(record.TextId, record.RuleId, record.Justification));
            }
            return violations;
        });
    }
}
