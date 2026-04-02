using System.Text.Json;
using System.Text.RegularExpressions;
using GridModel = StitchingDesigner.Models.GridModel;

namespace StitchingDesigner.Services;

public class JsonGridStorageService : IGridStorageService
{
    private const string PatternsDirectory = "Patterns";
    private readonly Regex fileNameRegex = new Regex(@"^[a-z0-9_-]+$", RegexOptions.IgnoreCase);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(string patternName, GridModel grid, CancellationToken cancellationToken = default)
    {
        var filePath = GetPatternPath(patternName);
        EnsurePatternsDirectoryExists();

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, grid, SerializerOptions, cancellationToken);
    }

    public async Task<GridModel?> LoadAsync(string patternName, CancellationToken cancellationToken = default)
    {
        var filePath = GetPatternPath(patternName);
        if (!File.Exists(filePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<GridModel>(stream, SerializerOptions, cancellationToken);
    }

    public string GetPatternPath(string patternName)
    {
        var patternsFolder = Path.Combine(FileSystem.AppDataDirectory, PatternsDirectory);
        return Path.Combine(patternsFolder, $"{patternName}.json");
    }

    public bool IsNameValid(string patternName)
    {
        return !patternName.IsWhiteSpace() && fileNameRegex.IsMatch(patternName);
    }

    private static void EnsurePatternsDirectoryExists()
    {
        var patternsFolder = Path.Combine(FileSystem.AppDataDirectory, PatternsDirectory);
        if (!Directory.Exists(patternsFolder))
        {
            Directory.CreateDirectory(patternsFolder);
        }
    }
}
