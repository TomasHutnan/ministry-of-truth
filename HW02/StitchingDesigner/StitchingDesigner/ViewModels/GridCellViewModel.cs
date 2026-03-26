using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;
using StitchingDesigner.Models;

namespace StitchingDesigner.ViewModels;

public partial class GridCellViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int Row { get; set; }

    [ObservableProperty]
    public partial int Col { get; set; }

    [ObservableProperty]
    public partial Floss? Floss { get; set; }

    public Color DisplayColor => TryGetColor(Floss, out var color) ? color : Colors.White;

    public string Label => Floss?.Id ?? string.Empty;

    public GridCellViewModel(int row, int col, Floss? floss = null)
    {
        Row = row;
        Col = col;
        Floss = floss;
    }

    partial void OnFlossChanged(Floss? value)
    {
        OnPropertyChanged(nameof(DisplayColor));
        OnPropertyChanged(nameof(Label));
    }

    private static bool TryGetColor(Floss? floss, out Color color)
    {
        color = Colors.White;

        if (floss is null || string.IsNullOrWhiteSpace(floss.Hex))
        {
            return false;
        }

        var hex = floss.Hex.StartsWith('#') ? floss.Hex : $"#{floss.Hex}";

        try
        {
            color = Color.FromArgb(hex);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
