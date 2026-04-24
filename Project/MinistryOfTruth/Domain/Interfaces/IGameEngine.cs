namespace MinistryOfTruth.Domain.Interfaces;

public interface IGameEngine
{
    public Task InitializeAsync();
    public void StartGame();
}
