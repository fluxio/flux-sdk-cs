namespace Flux.SDK.DataTableAPI
{
    /// <summary>Represents metadata information of the cell</summary>
    /// <remarks>Relevant to services which support the METADATA capability</remarks>
    /// <remarks>All cellInfo structures will include a Metadata field.</remarks>
    public class CellMetadata
    {
        /// <summary>Information about the cell creation</summary>
        /// <remarks>Currently the only field populated in the CellEvent for Create is the Time.</remarks>
        public CellEvent Create { get; set; }

        /// <summary>Information about the cell modification</summary>
        public CellEvent Modify { get; set; }
    }
}
