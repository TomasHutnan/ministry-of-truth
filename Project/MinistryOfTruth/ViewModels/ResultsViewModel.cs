using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.ViewModels.Base;
using MinistryOfTruth.ViewModels.Interfaces;

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
        SurvivedDaysText = $"{gameState.Day} DAYS";
        Score = scoreResult.Score;
        _ = InitializeAsync(highScoreStore, scoreResult.Score);
    }

    private async Task InitializeAsync(IHighScoreStore highScoreStore, int score)
    {
        if (await highScoreStore.SaveIfGreaterAsync(score))
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                NewHighScoreText = "NEW HIGH-SCORE");
        }

        await Task.Delay(500);
        await MainThread.InvokeOnMainThreadAsync(() =>
            IsContinueButtonEnabled = true);
    }

    [RelayCommand]
    public async Task Continue()
    {
        await _navigationService.GoToMenuAsync();
    }
}
