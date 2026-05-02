using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;
using MinistryOfTruth.ViewModels.Interfaces;
using System.Diagnostics;

namespace MinistryOfTruth.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    private ITextTicker _ticker;
    private IPopupService _popupService;

    [ObservableProperty]
    public partial string MenuTickerText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int HighScore { get; set; }

    public MainMenuViewModel(
        INavigationService navigationService,
        IGameEngine gameEngine,
        ITextTicker ticker,
        IHighScoreStore highScoreStore,
        IPopupService popupService) : base(navigationService, gameEngine)
    {
        _ticker = ticker;
        _ticker.TextUpdated += TickerTextUpdated;

        _popupService = popupService;

        Task.Run(() => HighScore = highScoreStore.LoadAsync().Result);
    }

    [RelayCommand]
    public async Task Play()
    {
        await _navigationService.GoToGameAsync();
        await _gameEngine.StartGameLoop();
    }

    [RelayCommand]
    public async Task LoadTextSet()
    {
        string? textSetPath = await _popupService.ShowInputAsync("Input the path to your text set archive.");
        if (textSetPath != null)
        {
            Debug.WriteLine($"Loading not yet implemented. Received path: {textSetPath}");
        }
    }

    [RelayCommand]
    public async Task ResetToDefaults()
    {
        bool confirmed = await _popupService.ShowConfirmationAsync("Are you sure? This action will overwrite the currently loaded text set!");
        if (confirmed)
        {
            Debug.WriteLine("Reset to defaults not yet implemented.");
        }
    }

    public async Task StartMenuTickerAsync()
    {
        await _ticker.StartTickerAsync();
    }

    public void StopMenuTicker()
    {
        _ticker.StopTicker();
    }

    private void TickerTextUpdated(object? sender, string newText)
    {
        MenuTickerText = newText;
    }
}
