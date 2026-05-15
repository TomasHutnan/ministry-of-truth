namespace MinistryOfTruth.Domain.Interfaces;

public interface ITickerTextSource
{
    Task<string> LoadTickerTextAsync(CancellationToken cancellationToken = default);
}
