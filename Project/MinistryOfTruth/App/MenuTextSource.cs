using MinistryOfTruth.ViewModels.Interfaces;

namespace App;

public class MenuTextSource : IMenuTextSource
{
    public async Task<string> LoadMenuTextAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync("menu_text.txt");
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
