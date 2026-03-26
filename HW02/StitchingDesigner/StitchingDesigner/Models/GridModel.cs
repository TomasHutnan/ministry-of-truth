using System.Text.Json.Serialization;

namespace StitchingDesigner.Models
{
    public readonly record struct GridModel
    {
        [JsonConstructor]
        public GridModel(int rowCount, int columnCount, CellModel[] cells)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            Cells = cells is null ? [] : (CellModel[])cells.Clone();
        }

        public int RowCount { get; init; }
        public int ColumnCount { get; init; }
        public CellModel[] Cells { get; init; }
    }
}
