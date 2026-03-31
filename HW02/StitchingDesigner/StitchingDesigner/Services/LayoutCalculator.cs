namespace StitchingDesigner.Services;

/// <summary>
/// Calculates optimal grid layout dimensions based on available space and grid configuration.
/// </summary>
public class LayoutCalculator
{
    /// <summary>
    /// Fits the grid into the available space by distributing space evenly across rows and columns.
    /// </summary>
    public LayoutResult Calculate(int rowCount, int columnCount, double availableWidth, double availableHeight, double defaultCellSize)
    {
        if (rowCount <= 0 || columnCount <= 0)
        {
            return new LayoutResult(defaultCellSize, 0, 0);
        }

        var widthBound = availableWidth > 0 ? Math.Floor(availableWidth / columnCount) : double.MaxValue;
        var heightBound = availableHeight > 0 ? Math.Floor(availableHeight / rowCount) : double.MaxValue;
        var boundedCellSize = Math.Min(widthBound, heightBound);

        if (boundedCellSize == double.MaxValue)
        {
            boundedCellSize = defaultCellSize;
        }

        var cellSize = Math.Max(1, boundedCellSize);
        var gridWidth = cellSize * columnCount;
        var gridHeight = cellSize * rowCount;

        return new LayoutResult(cellSize, gridWidth, gridHeight);
    }
}

public readonly record struct LayoutResult(double CellSize, double GridWidth, double GridHeight);
