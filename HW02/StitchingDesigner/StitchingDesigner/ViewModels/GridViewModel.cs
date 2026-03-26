using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StitchingDesigner.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StitchingDesigner.ViewModels
{
    public partial class GridViewModel : ObservableObject
    {
        private double _availableWidth;
        private double _availableHeight;

        [ObservableProperty]
        public partial ObservableCollection<GridCellViewModel> Cells { get; set; } = new();

        [ObservableProperty]
        public partial int RowCount { get; set; }

        [ObservableProperty]
        public partial int ColumnCount { get; set; }

        private double _cellSize = 32;
        public double CellSize
        {
            get => _cellSize;
            set => SetProperty(ref _cellSize, value);
        }

        private double _gridWidth;
        public double GridWidth
        {
            get => _gridWidth;
            set => SetProperty(ref _gridWidth, value);
        }

        private double _gridHeight;
        public double GridHeight
        {
            get => _gridHeight;
            set => SetProperty(ref _gridHeight, value);
        }

        public int GridSpan => Math.Max(1, ColumnCount);

        public PalleteViewModel Pallete { get; } = new();

        public GridViewModel()
        {
            UpdateSize(10, 10);
        }

        partial void OnColumnCountChanged(int value)
        {
            OnPropertyChanged(nameof(GridSpan));
            RecalculateCellSize();
        }

        partial void OnRowCountChanged(int value)
        {
            RecalculateCellSize();
        }

        public void UpdateAvailableSpace(double availableWidth, double availableHeight)
        {
            _availableWidth = availableWidth;
            _availableHeight = availableHeight;
            RecalculateCellSize();
        }

        [RelayCommand]
        private void CellClicked(GridCellViewModel? cell)
        {
            Debug.WriteLineIf(cell is not null, $"Clicked on row {cell!.Row}, col {cell!.Col}");
            cell?.Floss = Pallete.SelectedFloss;
        }

        public void UpdateSize(int rows, int cols)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(rows);
            ArgumentOutOfRangeException.ThrowIfNegative(cols);

            var existingFloss = Cells.ToDictionary(cell => (cell.Row, cell.Col), cell => cell.Floss);
            var resized = new ObservableCollection<GridCellViewModel>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    existingFloss.TryGetValue((r, c), out var floss);
                    var cell = new GridCellViewModel(r, c, floss);
                    resized.Add(cell);
                }
            }

            RowCount = rows;
            ColumnCount = cols;
            Cells = resized;
            RecalculateCellSize();
        }

        private void RecalculateCellSize()
        {
            if (RowCount <= 0 || ColumnCount <= 0)
            {
                return;
            }

            var widthBound = _availableWidth > 0 ? Math.Floor(_availableWidth / ColumnCount) : double.MaxValue;
            var heightBound = _availableHeight > 0 ? Math.Floor(_availableHeight / RowCount) : double.MaxValue;
            var boundedCellSize = Math.Min(widthBound, heightBound);

            if (boundedCellSize == double.MaxValue)
            {
                boundedCellSize = CellSize;
            }

            CellSize = Math.Max(1, boundedCellSize);
            GridWidth = CellSize * ColumnCount;
            GridHeight = CellSize * RowCount;
        }
    }
}
