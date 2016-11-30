namespace Flux.SDK.DataTableAPI
{
    /// <summary>Represents information about the cell</summary>
    public class CellInfo
    {
        /// <summary> Id of the cell.</summary>
        public string CellId { get; set; }

        /// <summary> Metadata of the cell</summary>
        /// <remarks> Only if METADATA capability is supported.</remarks>
        public CellMetadata Metadata { get; set; }

        /// <summary> Metadata of the client</summary>
        /// <remarks> Only if CLIENT_METADATA capability is supported.</remarks>
        public ClientMetadata ClientMetadata { get; set; }

        /// <summary>Determines whether the specified CellInfo is equal to the current System.Object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CellInfo)
                return Equals((CellInfo)obj);

            return false;
        }

        /// <summary>Determines whether the specified CellInfo is equal to the current CellInfo.</summary>
        /// <param name="cellInfo">The CellInfo to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(CellInfo cellInfo)
        {
            return CellId == cellInfo.CellId;
        }

        /// <summary>Serves as a hash function for the CellInfo type.</summary>
        /// <returns>A hash code for the current CellInfo.</returns>
        public override int GetHashCode()
        {
            if (CellId != null)
                return CellId.GetHashCode();
            return base.GetHashCode();
        }

        /// <summary>Determines whether two specified CellInfo instances are equal.</summary>
        /// <param name="a">The first CellInfo instance.</param>
        /// <param name="b">The second CellInfo instance.</param>
        /// <returns>True if the specified CellInfo instances are equal; otherwise, false.</returns>
        public static bool operator ==(CellInfo a, CellInfo b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        /// <summary>Determines whether two specified CellInfo instances are not equal.</summary>
        /// <param name="a">The first CellInfo instance.</param>
        /// <param name="b">The second CellInfo instance.</param>
        /// <returns>True if the specified CellInfo instances are not equal; otherwise, false.</returns>
        public static bool operator !=(CellInfo a, CellInfo b)
        {
            return !(a == b);
        }
    }
}