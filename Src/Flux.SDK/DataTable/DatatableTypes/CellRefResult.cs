namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Structure contains cell reference.</summary>
    public struct CellRefResult
    {
        /// <summary>Information about the cell</summary>
        public CellInfo Info { get; set; }

        /// <summary>The cell value reference</summary>
        public string ValueReference { get; set; }
    }
}