using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Interfaces;

public interface IGameEngine
{
    public event EventHandler<EventArgs>? GameStarted;
    public event EventHandler<ScoreResult>? GameEnded;
    public event EventHandler<GameState>? GameStateChanged;

    public Task StartGameLoop();

    public void Approve();
    public void Censor();
}
