namespace MinistryOfTruth.Domain.Interfaces;

public interface IHighScoreStore
{
    Task<int> LoadAsync();
    Task SaveAsync(int score);
}
