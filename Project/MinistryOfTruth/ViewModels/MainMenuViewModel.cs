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
    private ITextSetLoader _textSetLoader;
    private ITextRepository _textRepository;

    [ObservableProperty]
    public partial string MenuTickerText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int HighScore { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; } = false;

    public MainMenuViewModel(
        INavigationService navigationService,
        IGameEngine gameEngine,
        ITextTicker ticker,
        IHighScoreStore highScoreStore,
        IPopupService popupService,
        ITextSetLoader textSetLoader,
        ITextRepository textRepository) : base(navigationService, gameEngine)
    {
        _ticker = ticker;
        _ticker.TextUpdated += TickerTextUpdated;

        _popupService = popupService;
        _textSetLoader = textSetLoader;
        _textRepository = textRepository;

        Task.Run(() => HighScore = highScoreStore.LoadAsync().Result);
    }

    [RelayCommand]
    public async Task Play()
    {
        await _navigationService.GoToGameAsync();
    }

    [RelayCommand]
    public async Task LoadTextSet()
    {
        string? textSetPath = await _popupService.ShowInputAsync("Input the path to your text set archive.");
        if (textSetPath != null)
        {
            try
            {
                IsLoading = true;
                await _textSetLoader.LoadFromFileAsync(textSetPath);
                await _popupService.ShowNoticeAsync("Text set loaded successfully!");
            }
            catch (FileNotFoundException)
            {
                await _popupService.ShowErrorAsync("Error: File not found at the specified path.");
            }
            catch (InvalidOperationException ex)
            {
                await _popupService.ShowErrorAsync($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error loading text set: {ex}");
                await _popupService.ShowErrorAsync("Error: Failed to load text set. Check the archive format.");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    public async Task ResetToDefaults()
    {
        bool confirmed = await _popupService.ShowConfirmationAsync("Are you sure? This action will overwrite the currently loaded text set!");
        if (confirmed)
        {
            try
            {
                IsLoading = true;
                await _textSetLoader.ResetToDefaultAsync();
                await _popupService.ShowNoticeAsync("Text set reset to defaults successfully!");
            }
            catch (InvalidOperationException ex)
            {
                await _popupService.ShowErrorAsync($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error resetting text set: {ex}");
                await _popupService.ShowErrorAsync("Error: Failed to reset text set.");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public async Task StartMenuTickerAsync()
    {
        await _ticker.StartTickerAsync();
    }

    public async Task InitializeAsync()
    {
        await StartMenuTickerAsync();
        
        if (!_textRepository.RepositoryExists())
        {
            try
            {
                IsLoading = true;
                await _textSetLoader.LoadDefaultAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading default text set: {e.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
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
