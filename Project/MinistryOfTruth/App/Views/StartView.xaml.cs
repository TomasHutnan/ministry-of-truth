using MinistryOfTruth.ViewModels;

namespace App.Views;

public partial class StartView : ContentView
{
    public StartView(StartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}