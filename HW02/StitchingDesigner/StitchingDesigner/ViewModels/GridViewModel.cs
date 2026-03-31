using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StitchingDesigner.Models;
using StitchingDesigner.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StitchingDesigner.ViewModels
{
    public partial class GridViewModel : ObservableObject
    {
        private readonly IGridStorageService _gridStorageService;
        private readonly LayoutCalculator _layoutCalculator;
        private double _availableWidth;
        private double _availableHeight;

        [ObservableProperty]
        public partial ObservableCollection<GridCellViewModel> Cells { get; set; } = new();

        [ObservableProperty]
        public partial int RowCount { get; set; }

        [ObservableProperty]
        public partial int ColumnCount { get; set; }

        [ObservableProperty]
        public partial int EntryRowCount { get; set; }

        [ObservableProperty]
        public partial int EntryColumnCount { get; set; }

        [ObservableProperty]
        public partial string PatternName { get; set; } = "pattern";

        [ObservableProperty]
        public partial string LastSavedFilePath { get; set; } = "";

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

        public PalleteViewModel Pallete { get; }

        public GridViewModel() : this(new JsonGridStorageService(), new PalleteViewModel(), new LayoutCalculator())
        {
        }

        internal GridViewModel(IGridStorageService gridStorageService, PalleteViewModel pallete)
            : this(gridStorageService, pallete, new LayoutCalculator())
        {
        }

        internal GridViewModel(IGridStorageService gridStorageService, PalleteViewModel pallete, LayoutCalculator layoutCalculator)
        {
            _gridStorageService = gridStorageService;
            _layoutCalculator = layoutCalculator;
            Pallete = pallete;

            EntryRowCount = 10;
            EntryColumnCount = 15;
            SetSize();
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
            cell?.Floss = Pallete.SelectedFloss;
        }

        [RelayCommand]
        private void SetSize()
        {
            UpdateSize(EntryRowCount, EntryColumnCount);
        }

        [RelayCommand]
        private async Task SavePatternAsync()
        {
            var gridModel = new GridModel(
                RowCount,
                ColumnCount,
                Cells.Where(c => c.Floss is not null)
                     .Select(c => new CellModel(c.Floss!.Id, c.Row, c.Col))
                     .ToArray());

            await _gridStorageService.SaveAsync(PatternName, gridModel);
            LastSavedFilePath = Path.Combine(FileSystem.AppDataDirectory, "Patterns", $"{PatternName}.json");
        }

        [RelayCommand]
        private async Task LoadPatternAsync()
        {
            var loaded = await _gridStorageService.LoadAsync(PatternName);
            if (loaded is null)
            {
                return;
            }

            UpdateSize(0, 0);  // Deletes grid content
            EntryRowCount = loaded.Value.RowCount;
            EntryColumnCount = loaded.Value.ColumnCount;
            UpdateSize(loaded.Value.RowCount, loaded.Value.ColumnCount);

            foreach (var cell in loaded.Value.Cells)
            {
                var floss = Pallete.GetFlossById(cell.FlossId);
                if (floss is not null && cell.Row >= 0 && cell.Row < RowCount && cell.Col >= 0 && cell.Col < ColumnCount)
                {
                    var index = (cell.Row * ColumnCount) + cell.Col;
                    Cells[index].Floss = floss;
                }
            }
        }

        private void UpdateSize(int rows, int cols)
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
            var layout = _layoutCalculator.Calculate(RowCount, ColumnCount, _availableWidth, _availableHeight, _cellSize);
            CellSize = layout.CellSize;
            GridWidth = layout.GridWidth;
            GridHeight = layout.GridHeight;
        }
    }
}
