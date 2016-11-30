using Flux.Logger;
using System;
using Flux.SDK.Types;

namespace Flux.SDK.DataTableAPI
{
    /// <summary> Represents information about the cell event</summary>
    /// <remarks> Relevant to services which support the METADATA capability</remarks>
    public struct CellEvent
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|Datatable|CellEvent");

        /// <summary> The client id</summary>
        public string ClientId { get; set; }

        /// <summary> The client information</summary>
        public ClientInfo ClientInfo { get; set; }

        /// <summary> The time when event occured</summary>
        public double Time { get; set; }

        /// <summary>size of the value stored in this cell</summary>
        public Int64 Size { get; set; }

        /// <summary> The size of the value stored in this cell</summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Converts Java timestamp to DateTime
        /// </summary>
        /// <returns>Returns time as DateTime structure</returns>
        public DateTime GetDate()
        {
            DateTime dt = default(DateTime);

            try
            {
                dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Math.Round(Time / 1000d)).ToLocalTime();
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }

            return dt;
        }
    }
}