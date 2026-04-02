using System.Diagnostics;

namespace StitchingDesigner.Services;

public static class LayoutCalculator
{
    private const double MinimumCellSize = 40;

    public static LayoutResult Calculate(int rowCount, int columnCount, double availableWidth, double availableHeight, double defaultCellSize)
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

        var cellSize = Math.Max(MinimumCellSize, boundedCellSize);
        var gridWidth = cellSize * columnCount;
        var gridHeight = cellSize * rowCount;

        Debug.WriteLine($"{cellSize}, {gridWidth}, {gridHeight}");

        return new LayoutResult(cellSize, gridWidth, gridHeight);
    }
}

public readonly record struct LayoutResult(double CellSize, double GridWidth, double GridHeight);
