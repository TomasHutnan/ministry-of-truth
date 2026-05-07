using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Domain.Presentation;
using MinistryOfTruth.ViewModels.Base;
using System.Diagnostics;

namespace MinistryOfTruth.ViewModels;

public partial class GameViewModel: ViewModelBase
{
    private IGameEngine _engine;
    private RuleTextFormatter _ruleTextFormatter;

    [ObservableProperty]
    public partial GameState GameState { get; set; } = new GameState("", "", 0, 0, 0, 0);

    [ObservableProperty]
    public partial string RuleLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string DayLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string ScoreLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string TextsRemainingLabelText { get; set; } = "";

    [ObservableProperty]
    public partial double FillBarWidth { get; set; } = 0;

    [ObservableProperty]
    public partial double ContainerWidth { get; set; } = 0;

    partial void OnContainerWidthChanged(double value)
    {
        UpdateFillBar();
    }

    [RelayCommand]
    public void Approve()
    {
        _engine.Approve();
    }

    [RelayCommand]
    public void Censor()
    {
        _engine.Censor();
    }

    public GameViewModel(
        INavigationService navigationService,
        IGameEngine gameEngine,
        RuleTextFormatter ruleTextFormatter) : base(navigationService, gameEngine)
    {
        _engine = gameEngine;
        _ruleTextFormatter = ruleTextFormatter;

        Task.Run(_gameEngine.StartGameLoop);
        _engine.GameStateChanged += GameStateChanged;
        _engine.GameEnded += GameEndend;
    }

    ~GameViewModel()
    {
        _engine.GameStateChanged -= GameStateChanged;
        _engine.GameEnded -= GameEndend;
    }

    private void GameStateChanged(object? sender, GameState newGameState)
    {
        if (GameState.Rule != newGameState.Rule)
        {
            RuleLabelText = _ruleTextFormatter.BuildRuleText(newGameState.Rule);
        }
        if (GameState.Day != newGameState.Day)
        {
            DayLabelText = $"Day {newGameState.Day}";
        }
        if (GameState.Score != newGameState.Score)
        {
            ScoreLabelText = $"{newGameState.Score} PTS";
        }
        if (GameState.TextsRemaining != newGameState.TextsRemaining)
        {
            string pluralS = newGameState.TextsRemaining == 1 ? "" : "s";
            TextsRemainingLabelText = $"{newGameState.TextsRemaining} text{pluralS} remaining";
        }
        if (newGameState.IsCorrectDecision)
        {
            // TODO
        }
        if (GameState.StatusMessage != newGameState.StatusMessage)
        {
            // TODO
        }

        GameState = newGameState;
        UpdateFillBar();
    }

    public void UpdateFillBar()
    {
        double ratio = GameState?.DangerLevelRatio ?? 0d;
        double available = ContainerWidth;

        // Ensure ratio is within [0,1]
        if (double.IsNaN(ratio) || double.IsInfinity(ratio)) ratio = 0d;
        if (ratio < 0d) ratio = 0d;
        if (ratio > 1d) ratio = 1d;

        double newWidth = available * ratio;
        FillBarWidth = newWidth;
    }

    private async void GameEndend(object? sender, ScoreResult scoreResult)
    {
        await _navigationService.GoToResultsAsync(GameState, scoreResult);
    }
}
