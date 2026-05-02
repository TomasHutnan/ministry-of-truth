using CommunityToolkit.Mvvm.ComponentModel;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.ViewModels.Base;

namespace MinistryOfTruth.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    private ITextTicker _ticker;

    [ObservableProperty]
    public partial string MenuTickerText { get; set; } = string.Empty;

    public MainMenuViewModel(
        INavigationService navigationService,
        IGameEngine gameEngine,
        ITextTicker ticker) : base(navigationService, gameEngine)
    {
        _ticker = ticker;
        _ticker.TextUpdated += TickerTextUpdated;
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
