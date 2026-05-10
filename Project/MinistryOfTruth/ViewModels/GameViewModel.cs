using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Domain.Presentation;
using MinistryOfTruth.ViewModels.Base;
using System.Diagnostics;
using MinistryOfTruth.ViewModels.Interfaces;

namespace MinistryOfTruth.ViewModels;

public partial class GameViewModel: ViewModelBase, IDisposable
{
    private IGameEngine _engine;
    private RuleTextFormatter _ruleTextFormatter;
    private bool _disposed = false;

    private Color _red = Colors.Red;
    private Color _green = Colors.Green;

    [ObservableProperty]
    public partial GameState GameState { get; set; } = new GameState("", "", 0, 0, 0, 0.0, true, "");

    [ObservableProperty]
    public partial string RuleLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string DayLabelText { get; set; } = "";

    [ObservableProperty]
    public partial string ScoreLabelText { get; set; } = "";

    [ObservableProperty]
    public partial Color ScoreLabelColor { get; set; } = Colors.Green;

    [ObservableProperty]
    public partial string TextsRemainingLabelText { get; set; } = "";

    [ObservableProperty]
    public partial bool FlashIncorrect { get; set; } = false;

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

        if (Application.Current != null)
        {
            if (Application.Current.Resources.TryGetValue("ViolationRed", out var colorvalue))
            {
                _red = (Color)colorvalue;
            }
            if (Application.Current.Resources.TryGetValue("MinistryGreen", out colorvalue))
            {
                _green = (Color)colorvalue;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            _engine.GameStateChanged -= GameStateChanged;
            _engine.GameEnded -= GameEndend;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during GameViewModel disposal: {ex.Message}");
        }
        finally
        {
            _disposed = true;
        }
    }

    ~GameViewModel()
    {
        Dispose();
    }

    private void GameStateChanged(object? sender, GameState newGameState)
    {
        if (_disposed) return;

        if (GameState?.Rule != newGameState.Rule)
        {
            RuleLabelText = _ruleTextFormatter.BuildRuleText(newGameState.Rule);
        }
        if (GameState?.Day != newGameState.Day)
        {
            DayLabelText = $"Day {newGameState.Day}";
        }

        if (GameState?.Score < newGameState.Score)
        {
            ScoreLabelText = $"↑ {newGameState.Score} PTS";
            ScoreLabelColor = _green;
        }
        else if (GameState?.Score > newGameState.Score)
        {
            ScoreLabelText = $"↓ {newGameState.Score} PTS";
            ScoreLabelColor = _red;
        }

        if (GameState?.TextsRemaining != newGameState.TextsRemaining)
        {
            string pluralS = newGameState.TextsRemaining == 1 ? "" : "s";
            TextsRemainingLabelText = $"{newGameState.TextsRemaining} text{pluralS} remaining";
        }
        
        if (newGameState.IsCorrectDecision == false)
        {
            Debug.WriteLine("Flashing red.");
            MainThread.BeginInvokeOnMainThread(() => 
            {
                if (!_disposed)
                {
                    FlashIncorrect = true;
                }
            });
        }

        if (GameState?.StatusMessage != newGameState.StatusMessage)
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

        if (double.IsNaN(ratio) || double.IsInfinity(ratio)) ratio = 0d;
        if (ratio < 0d) ratio = 0d;
        if (ratio > 1d) ratio = 1d;

        double newWidth = Math.Max(0, available) * ratio;
        FillBarWidth = newWidth;
    }

    private async void GameEndend(object? sender, ScoreResult scoreResult)
    {
        if (_disposed) return;
        await _navigationService.GoToResultsAsync(GameState, scoreResult);
    }
}
