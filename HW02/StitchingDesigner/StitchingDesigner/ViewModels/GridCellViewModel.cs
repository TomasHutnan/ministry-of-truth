using CommunityToolkit.Mvvm.ComponentModel;
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

    public string Label => Floss?.Id ?? string.Empty;

    public GridCellViewModel(int row, int col, Floss? floss = null)
    {
        Row = row;
        Col = col;
        Floss = floss;
    }

    partial void OnFlossChanged(Floss? value)
    {
        OnPropertyChanged(nameof(Label));
    }
}
