using MinistryOfTruth.Domain.Interfaces;

namespace App;

public class MenuTickerTextSource : ITickerTextSource
{
    public async Task<string> LoadTickerTextAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("menu_text.txt");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
