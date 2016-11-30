namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Represents a single cell</summary>
    public struct Cell
    {
        /// <summary>The cell information</summary>
        public CellInfo Info { get; set; }
        /// <summary>The cell value</summary>
        public CellValue Value { get; set; }
    }
}