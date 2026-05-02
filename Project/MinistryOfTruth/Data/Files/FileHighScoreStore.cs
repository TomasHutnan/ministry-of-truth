using MinistryOfTruth.Domain.Interfaces;
using System.Globalization;

namespace MinistryOfTruth.Data.Files;

public class FileHighScoreStore : IHighScoreStore
{
    private static readonly string _highScorePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "highscore.txt");

    public async Task<int> LoadAsync()
    {
        if (!File.Exists(_highScorePath))
        {
            return 0;
        }

        string content = await File.ReadAllTextAsync(_highScorePath);

        if (!int.TryParse(content.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int score))
        {
            return 0;
        }

        return score >= 0 ? score : 0;
    }

    public async Task SaveAsync(int score)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(score, 0, nameof(score));

        string directory = Path.GetDirectoryName(_highScorePath)!;
        Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(_highScorePath, score.ToString(CultureInfo.InvariantCulture));
    }

    public async Task SaveIfGreaterAsync(int score)
    {
        int currentScore = await LoadAsync();
        if (score > currentScore)
        {
            await SaveAsync(score);
        }
    }
}
