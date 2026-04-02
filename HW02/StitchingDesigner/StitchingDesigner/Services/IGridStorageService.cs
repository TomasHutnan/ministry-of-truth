using StitchingDesigner.Models;

namespace StitchingDesigner.Services;

public interface IGridStorageService
{
    Task SaveAsync(string patternName, GridModel grid, CancellationToken cancellationToken = default);
    Task<GridModel?> LoadAsync(string patternName, CancellationToken cancellationToken = default);
    string GetPatternPath(string patternName);
    bool IsNameValid(string patternName);
}
