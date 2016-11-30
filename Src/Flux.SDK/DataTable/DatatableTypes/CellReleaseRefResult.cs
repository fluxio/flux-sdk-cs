namespace Flux.SDK.DataTableAPI.DatatableTypes
{
    /// <summary>Contains dereferenced permanent cell reference to value</summary>
    public struct CellReleaseRefResult
    {
        /// <summary>The cell value</summary>
        public string Value { get; set; }

        /// <summary>The cell value metadata</summary>
        public CellEvent ValueMetadata { get; set; }

        /// <summary>The cell metadata</summary>
        public CellMetadata Metadata { get; set; }

        /// <summary>The cell client metadata</summary>
        public ClientMetadata ClientMetadata { get; set; }
    }
}