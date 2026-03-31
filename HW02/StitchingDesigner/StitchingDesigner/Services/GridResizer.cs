using StitchingDesigner.ViewModels;
using System.Collections.ObjectModel;

namespace StitchingDesigner.Services;

/// <summary>
/// Service for resizing grids while preserving existing cell data.
/// </summary>
public class GridResizer
{
    /// <summary>
    /// Resize a grid to new dimensions, preserving floss data where possible.
    /// </summary>
    /// <param name="currentCells">The current grid cells</param>
    /// <param name="newRowCount">Target number of rows</param>
    /// <param name="newColumnCount">Target number of columns</param>
    /// <returns>A new collection of cells with the specified dimensions</returns>
    /// <exception cref="ArgumentOutOfRangeException">If dimensions are negative</exception>
    public ObservableCollection<GridCellViewModel> Resize(
        IEnumerable<GridCellViewModel> currentCells,
        int newRowCount,
        int newColumnCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(newRowCount);
        ArgumentOutOfRangeException.ThrowIfNegative(newColumnCount);

        // Preserve existing floss by position
        var existingFloss = currentCells
            .ToDictionary(cell => (cell.Row, cell.Col), cell => cell.Floss);

        var resized = new ObservableCollection<GridCellViewModel>();

        // Create new grid, restoring floss where it existed
        for (int row = 0; row < newRowCount; row++)
        {
            for (int col = 0; col < newColumnCount; col++)
            {
                existingFloss.TryGetValue((row, col), out var floss);
                var cell = new GridCellViewModel(row, col, floss);
                resized.Add(cell);
            }
        }

        return resized;
    }
}
