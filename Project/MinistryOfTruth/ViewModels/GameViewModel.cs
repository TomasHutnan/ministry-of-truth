using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Domain.Presentation;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public partial class GameViewModel: ViewModelBase
{
    private IGameEngine _engine;
    private RuleTextFormatter _ruleTextFormatter;

    [ObservableProperty]
    public partial GameState GameState { get; set; } = new GameState("", "", 0, 0, 0, 0, 0);

    [ObservableProperty]
    public partial string RuleLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string DayLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string TextsRemainingLabelText { get; set; } = "";

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
    }

    private async void GameEndend(object? sender, ScoreResult scoreResult)
    {
        await _navigationService.GoToResultsAsync(GameState, scoreResult);
    }
}
