using MinistryOfTruth.ViewModels;
using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace App.Views;

public partial class GameView : ContentView
{
    private GameViewModel? _vm;

    public GameView(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        BindingContextChanged += OnBindingContextChanged;
        OnBindingContextChanged(this, EventArgs.Empty);

        // Ensure container width is set after initial layout pass
        Loaded += async (s, e) =>
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await System.Threading.Tasks.Task.Delay(50);
                if (BindingContext is GameViewModel vm && FillContainer != null)
                {
                    vm.ContainerWidth = Math.Max(0, FillContainer.Width);
                }
            });
        };

        // Also update when size changes
        SizeChanged += (s, e) =>
        {
            if (BindingContext is GameViewModel vm && FillContainer != null)
            {
                vm.ContainerWidth = Math.Max(0, FillContainer.Width);
            }
        };

        Unloaded += (s, e) => _vm?.Dispose();
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        _vm?.PropertyChanged -= VmOnPropertyChanged;
        _vm = BindingContext as GameViewModel;
        _vm?.PropertyChanged += VmOnPropertyChanged;
    }

    private void VmOnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.FlashIncorrect) && _vm != null)
        {
            if (_vm.FlashIncorrect)
            {
                if (IncorrectFlash == null)
                    return;

                Dispatcher.Dispatch(async () =>
                {
                    try
                    {
                        // quick flash: fade in then out
                        await IncorrectFlash.FadeToAsync(0.6, 80);
                        await IncorrectFlash.FadeToAsync(0, 160);
                    }
                    catch
                    {
                        // ignore animation errors
                    }
                    finally
                    {
                        // reset flag on main thread
                        MainThread.BeginInvokeOnMainThread(() => _vm.FlashIncorrect = false);
                    }
                });
            }
        }
    }
}