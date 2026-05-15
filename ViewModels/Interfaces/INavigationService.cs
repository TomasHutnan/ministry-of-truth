using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.ViewModels.Interfaces;

public interface INavigationService
{
    Task GoToStartAsync();
    Task GoToMenuAsync();
    Task GoToGameAsync();
    Task GoToResultsAsync(GameState gameState, ScoreResult scoreResult);
}
