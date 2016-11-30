using System;
using System.Runtime.Serialization;

namespace Flux.SDK.DataTableAPI
{
    /// <summary>Represents the method that will handle the OnNotification event.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">NotificationEventArgs that contains notification data.</param>
    public delegate void NotificationEventHandler(object sender, NotificationEventArgs e);
    /// <summary>Represents the method that will handle the OnError event.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">ErrorEventArgs that contains error data.</param>
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    /// <summary>Contains the cell notification data</summary>
    [DataContract, System.Runtime.InteropServices.GuidAttribute("3A270E83-CE2B-45FE-945F-42A28DE8338A")]
    public class Notification
    {
        /// <summary>The cell information</summary>
        [DataMember(Name = "CellInfo")]
        public CellInfo CellInfo { get; set; }

        /// <summary>The cell event</summary>
        [DataMember(Name = "Event")]
        public CellEvent CellEvent { get; set; }
    }

    /// <summary>Represents error occured on datatable</summary>
    public class Error
    {
        /// <summary>Initializes new Error instance</summary>
        /// <param name="message">The error message</param>
        public Error(string message)
        {
            Message = message;
        }

        /// <summary>The error message</summary>
        public string Message { get; set; }
    }

    /// <summary>Represents the class that contain data for the Notification received event</summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>The type of notification</summary>
        public Notification Notification { get; set; }
        /// <summary>The project id</summary>
        public string ProjectId { get; set; }

        /// <summary>Initializes new NotificationEventArgs instance</summary>
        /// <param name="notification">The type of notification</param>
        /// <param name="projectId">The project id</param>
        public NotificationEventArgs(Notification notification, string projectId)
        {
            Notification = notification;
            ProjectId = projectId;
        }
    }

    /// <summary>Represents the class that contain data for the  Error occured event</summary>
    public class ErrorEventArgs : EventArgs
    {
        ///<summary>The occured Error.</summary>
        public Error Error { get; set; }

        /// <summary>The project id.</summary>
        public string ProjectId { get; set; }

        /// <summary>Initializes new ErrorEventArgs instance.</summary>
        /// <param name="error">The occured Error.</param>
        /// <param name="projectId">The project id.</param>
        public ErrorEventArgs(Error error, string projectId)
        {
            Error = error;
            ProjectId = projectId;
        }
    }
}
