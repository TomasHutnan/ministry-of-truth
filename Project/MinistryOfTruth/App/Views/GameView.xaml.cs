using MinistryOfTruth.ViewModels;

namespace App.Views;

public partial class GameView : ContentView
{
    public GameView(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}