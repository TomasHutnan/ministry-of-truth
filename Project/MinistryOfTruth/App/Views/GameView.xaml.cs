using MinistryOfTruth.ViewModels;
using System;

namespace App.Views;

public partial class GameView : ContentView
{
    public GameView(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Update VM with current container width when view size changes
        SizeChanged += OnSizeChanged;

        // Set initial width if already available
        if (FillContainer != null && BindingContext is GameViewModel vm)
        {
            vm.ContainerWidth = FillContainer.Width;
        }
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (FillContainer == null)
            return;

        if (BindingContext is GameViewModel vm)
        {
            vm.ContainerWidth = FillContainer.Width;
        }
    }
}