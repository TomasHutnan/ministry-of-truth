using MinistryOfTruth.Domain.Interfaces;

namespace App.Services;

public class MauiAppAssetProvider : IAppAssetProvider
{
    public async Task<Stream> OpenAssetAsync(string assetName)
    {
        ArgumentNullException.ThrowIfNull(assetName);
        return await FileSystem.OpenAppPackageFileAsync(assetName);
    }
}
