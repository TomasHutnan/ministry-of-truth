using MinistryOfTruth.ViewModels;

namespace App.Views;

public partial class MainMenuView : ContentView
{
    private readonly MainMenuViewModel _viewModel;

    public MainMenuView(MainMenuViewModel viewModel)
    {
        _viewModel = viewModel;

        InitializeComponent();
        BindingContext = viewModel;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await _viewModel.StartMenuTickerAsync();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        _viewModel.StopMenuTicker();
    }
}