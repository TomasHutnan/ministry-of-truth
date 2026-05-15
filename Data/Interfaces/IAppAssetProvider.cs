namespace MinistryOfTruth.Domain.Interfaces;

public interface IAppAssetProvider
{
    Task<Stream> OpenAssetAsync(string assetName);
}
