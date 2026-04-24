namespace MinistryOfTruth.ViewModels.Interfaces;

public interface IMenuTextSource
{
    Task<string> LoadMenuTextAsync(CancellationToken cancellationToken = default);
}
