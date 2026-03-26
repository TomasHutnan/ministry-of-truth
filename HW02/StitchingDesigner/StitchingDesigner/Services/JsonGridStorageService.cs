using System.Text.Json;
using GridModel = StitchingDesigner.Models.GridModel;

namespace StitchingDesigner.Services;

public class JsonGridStorageService : IGridStorageService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(string patternName, GridModel grid, CancellationToken cancellationToken = default)
    {
        var filePath = GetPatternPath(patternName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

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

    private static string GetPatternPath(string patternName)
    {
        var safeName = string.Join("_", patternName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "pattern";
        }

        return Path.Combine(FileSystem.AppDataDirectory, "Patterns", $"{safeName}.json");
    }
}
