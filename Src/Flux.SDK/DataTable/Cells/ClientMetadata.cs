namespace Flux.SDK.DataTableAPI
{
    /// <summary>Represents client metadata of the cell</summary>
    public class ClientMetadata
    {
        /// <summary>The cell label</summary>
        public string Label { get; set; }

        /// <summary>The cell description</summary>
        public string Description { get; set; }

        /// <summary>If set to true then the cell will be readonly</summary>
        public bool Locked { get; set; }
    }
}
