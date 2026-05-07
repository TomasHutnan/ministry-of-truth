using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public partial class ResultsViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string SurvivedDaysText { get; set; }

    [ObservableProperty]
    public partial string NewHighScoreText { get; set; } = "SCORE";

    [ObservableProperty]
    public partial int Score { get; set; }

    [ObservableProperty]
    public partial bool IsContinueButtonEnabled { get; set; } = false;

    public ResultsViewModel(
        INavigationService navigationService,
        IGameEngine gameEngine,
        IHighScoreStore highScoreStore,
        GameState gameState,
        ScoreResult scoreResult) : base(navigationService, gameEngine)
    {
        Task.Run(() =>
        {
            if (highScoreStore.SaveIfGreaterAsync(scoreResult.Score).Result)
            {
                NewHighScoreText = "NEW HIGH-SCORE";
            }
        });
        Task.Delay(2000).ContinueWith((_) => IsContinueButtonEnabled = true);
        SurvivedDaysText = $"{gameState.Day} DAYS";
        Score = scoreResult.Score;
    }

    [RelayCommand]
    public async Task Continue()
    {
        await _navigationService.GoToMenuAsync();
    }
}
