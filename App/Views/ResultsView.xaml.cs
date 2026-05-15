using MinistryOfTruth.ViewModels;

namespace App.Views;

public partial class ResultsView : ContentView
{
    public ResultsView(ResultsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}