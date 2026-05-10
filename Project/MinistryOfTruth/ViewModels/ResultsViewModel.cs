using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinistryOfTruth.Domain.Engine;
using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.ViewModels.Base;
using MinistryOfTruth.ViewModels.Interfaces;

namespace MinistryOfTruth.ViewModels;

public partial class ResultsViewModel : ViewModelBase, IDisposable
{
    private IHighScoreStore _highScoreStore;
    private CancellationTokenSource _initializationCancellation = new();
    private bool _disposed = false;

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
        _highScoreStore = highScoreStore ?? throw new ArgumentNullException(nameof(highScoreStore));
        SurvivedDaysText = $"{gameState.Day} DAYS";
        Score = scoreResult.Score;
        
        // Initialize without awaiting (fire-and-forget, but with proper cancellation tracking)
        _ = InitializeAsync(scoreResult.Score, _initializationCancellation.Token);
    }

    private async Task InitializeAsync(int score, CancellationToken cancellationToken)
    {
        try
        {
            if (await _highScoreStore.SaveIfGreaterAsync(score))
            {
                if (!cancellationToken.IsCancellationRequested && !_disposed)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (!_disposed)
                        {
                            NewHighScoreText = "NEW HIGH-SCORE";
                        }
                    });
                }
            }

            await Task.Delay(500, cancellationToken);
            
            if (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!_disposed)
                    {
                        IsContinueButtonEnabled = true;
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
            // View model was disposed before initialization completed
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ResultsViewModel initialization: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        try
        {
            _initializationCancellation?.Cancel();
            _initializationCancellation?.Dispose();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during ResultsViewModel disposal: {ex.Message}");
        }
        finally
        {
            _disposed = true;
        }
    }

    ~ResultsViewModel()
    {
        Dispose();
    }

    [RelayCommand]
    public async Task Continue()
    {
        await _navigationService.GoToMenuAsync();
    }
}
