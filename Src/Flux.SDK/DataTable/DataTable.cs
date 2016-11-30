using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Flux.SDK.WebServices;
using System.Threading.Tasks;
using Flux.SDK.DataTableAPI.DatatableTypes;
using Flux.Serialization;
using System.Runtime.Serialization;

namespace Flux.SDK.DataTableAPI
{
/// <summary>Represents data and operations with cells</summary>
    public class DataTable
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|Datatable");

        private string projectId;
        private SDKMetadata sdkMetadata;
        private List<FluxCookie> cookies;

        private Capability capability;
        private NotificationType subscribedNotificationTypes;
        private WebSocket webSocket;   
        
        private List<CellInfo> cells;

        /// <summary>Occurs when notification is received.</summary>
        public event NotificationEventHandler OnNotification;
        /// <summary>Occurs when error message received.</summary>
        public event ErrorEventHandler OnError;
        /// <summary>Occurs when WebSocket is reconnected.</summary>
        public Action OnWebSocketReconnected;

        internal DataTable(string projectId, List<FluxCookie> cookies, SDKMetadata sdkMetadata)
        {
            this.subscribedNotificationTypes = NotificationType.__NONE__;
            this.cookies = cookies;
            this.projectId = projectId;
            this.sdkMetadata = sdkMetadata;

            //init capability
            capability = GetCapabilities();

            //subscribe to datatable notifications
            SubscribeToAllNotifications();
        }
        
        internal WebSocket WebSocket
        {
            get
            {
                if (webSocket == null)
                {
                    webSocket = new WebSocket(projectId, cookies, sdkMetadata);
                    webSocket.OnReconnected += WebSocket_OnReconnected;
                    webSocket.OnNotificationMessage += OnNotificationMessage;
                    webSocket.OnErrorMessage += OnErrorMessage;
                }

                return webSocket;
            }
        }

        internal List<FluxCookie> Cookies
        {
            get
            {
                return cookies;
            }
            set
            {
                if (cookies == value)
                    return;

                cookies = value;

                if (WebSocket != null)
                    WebSocket.UserCookies = value;
            }
        }

        /// <summary>Provides ability to track the progress.</summary>
        [Obsolete("Use progress stream to track progress.")]
        public ProgressReporter ProgressTracker { get; set; }

        /// <summary>Returns datatable cells list.</summary>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public List<CellInfo> Cells
        {
            get
            {
                if (cells == null)
                    UpdateCells();

                return cells;
            }
        }

        /// <summary>Returns Capability of the datatable.</summary>
        public Capability Capability
        {
            get
            {
                return capability;
            }
        }

        #region Get cell value

        /// <summary>Get cell value stream by id.</summary>
        /// <param name="cellId">Id of the cell.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Stream of the cell value.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public Cell GetCell(string cellId, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var cell = new Cell();
            try
            {
                log.Info("Start GET request.");
                var getUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, getUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting response.");
                var response = HttpWebClientHelper.GetResponse(request);
                if (response != null)
                {
                    cell.Info = HttpWebClientHelper.ExtractAuxParams<CellInfo>(response);
                    var value = new CellValue();
                    var length = cell.Info.Metadata != null ? cell.Info.Metadata.Modify.Size : response.ContentLength;
                    var progressStream = new ProgressStream(StreamUtils.GetDecompressedResponseStream(response), length);
                    value.Stream = progressStream;
                    value.StreamLength = length;
                    cell.Value = value;
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cell;
        }

        /// <summary>Get cell value stream by id (Asyncronous method)./// </summary>
        /// <param name="cellId">Id of the cell.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>     
        /// <returns>Stream of the cell value.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<Cell> GetCellAsync(string cellId, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var cell = new Cell();
            try
            {
                log.Info("Start GET request.");
                var getUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, getUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting response.");
                var response = await HttpWebClientHelper.GetResponseAsync(request);
                if (response != null)
                {
                    cell.Info = HttpWebClientHelper.ExtractAuxParams<CellInfo>(response);
                    var value = new CellValue();
                    var length = cell.Info.Metadata != null ? cell.Info.Metadata.Modify.Size : response.ContentLength;
                    var progressStream = new ProgressStream(StreamUtils.GetDecompressedResponseStream(response), length);
                    value.Stream = progressStream;
                    value.StreamLength = length;
                    cell.Value = value;
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cell;
        }

        /// <summary>Get cell value by cells instance./// </summary>
        /// <param name="cell">CellInfo</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Stream of the cell value.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public Cell GetCell(CellInfo cell, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            if (cell == null)
                throw new Exceptions.InternalSDKException("Cell cannot be null.");

            return GetCell(cell.CellId, extractCellMetadata, extractClientMetadata);
        }

        /// <summary>Get cell value by cells instance (Asyncronous method)./// </summary>
        /// <param name="cell">CellInfo</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Stream of the cell value.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<Cell> GetCellAsync(CellInfo cell, bool extractCellMetadata = false,
            bool extractClientMetadata = false)
        {
            if (cell == null)
                throw new Exceptions.InternalSDKException("Cell cannot be null.");

            var getTask = await GetCellAsync(cell.CellId, extractCellMetadata, extractClientMetadata);
            return getTask;
        }

        #endregion

        #region Set cell value

        /// <summary>Updates cell value.</summary>
        /// <param name="cellId">Id of the cell to be updated.</param>
        /// <param name="valueStream">Value</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <param name="ignoreValue">If set to true then passed value is ignored.</param>
        /// <returns>Cell info</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly or deleted.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public CellInfo SetCell(string cellId, Stream valueStream, ClientMetadata clientMetadata = null,
            bool ignoreValue = false)
        {
            log.Info("Start SET request.");
            CellInfo cellInfo = null;

            try
            {
                var setUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, setUrl, cookies);
                request.Method = "POST";
                request.ContentType = "application/json";

                //update headers with opt params
                var optParams = new Dictionary<string, object>();
                if (clientMetadata != null)
                {
                    log.Info("Adding clientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                    {
                        optParams["ClientMetadata"] = clientMetadata;
                        if (ignoreValue)
                            optParams["IgnoreValue"] = ignoreValue;
                    }
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);

                log.Info("Writing value stream to request stream.");

                if (valueStream != null)
                {
                    var progressStream = new ProgressStream(valueStream);
                    progressStream.OnProgressChanged += ProgressStream_OnProgressChanged;
                    HttpWebClientHelper.WriteToRequest(request, progressStream);
                }

                log.Info("Start getting response.");

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    cellInfo = DataSerializer.Deserialize<CellInfo>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cellInfo;
        }

        /// <summary>Updates cell value.</summary>
        /// <param name="cell">CellInfo instance to be updated.</param>
        /// <param name="json">Json value to be written to the cell.</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <param name="ignoreValue">If set to true then passed value is ignored.</param>
        /// <returns>Cell info</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>        
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public CellInfo SetCell(CellInfo cell, string json, ClientMetadata clientMetadata = null, bool ignoreValue = false)
        {
            var serializedValueStream = StreamUtils.GenerateStreamFromString(json);
            var cellId = cell != null ? cell.CellId : "";
            return SetCell(cellId, serializedValueStream, clientMetadata, ignoreValue);
        }

        /// <summary>Updates cell value (Asynchronous method).</summary>
        /// <param name="cellId">Id of the cell to be updated.</param>
        /// <param name="valueStream">Stream of the value to be uploaded.</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <param name="ignoreValue">If set to true then passed value is ignored.</param>
        /// <returns>Cell info</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<CellInfo> SetCellAsync(string cellId, Stream valueStream, ClientMetadata clientMetadata = null, bool ignoreValue = false)
        {
            log.Info("Start SET request.");
            CellInfo cellInfo = null;

            try
            {
                var setUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) +
                             string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, setUrl, cookies);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.AllowWriteStreamBuffering = false;
                request.SendChunked = true;
                if (valueStream != null)
                    request.ContentLength = valueStream.Length;

                //update headers with opt params
                var optParams = new Dictionary<string, object>();
                if (clientMetadata != null)
                {
                    log.Info("Adding clientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                    {
                        optParams["ClientMetadata"] = clientMetadata;
                        if (ignoreValue)
                            optParams["IgnoreValue"] = ignoreValue;
                    }
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }

                request = HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);

                log.Info("Writing value stream to request stream.");
                if (valueStream != null)
                {
                    var progressStream = new ProgressStream(valueStream);
                    progressStream.OnProgressChanged += ProgressStream_OnProgressChanged;

                    await HttpWebClientHelper.WriteToRequestAsync(request, progressStream);
                }

                log.Info("Start getting response.");

                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    cellInfo = DataSerializer.Deserialize<CellInfo>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cellInfo;
        }

        /// <summary>Updates cell value (Asynchronous method).</summary>
        /// <param name="cell">CellInfo instance to be updated.</param>
        /// <param name="json">Json value to be written to the cell.</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <param name="ignoreValue">If set to true then passed value is ignored.</param>
        /// <returns>Cell info</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>       
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<CellInfo> SetCellAsync(CellInfo cell, string json, ClientMetadata clientMetadata = null, bool ignoreValue = false)
        {
            var serializedValueStream = StreamUtils.GenerateStreamFromString(json);
            var cellId = cell != null ? cell.CellId : "";
            var setTask = await SetCellAsync(cellId, serializedValueStream, clientMetadata, ignoreValue);
            return setTask;
        }

        #endregion

        #region Create cell

        /// <summary>Create new cell.</summary>
        /// <param name="value">Value</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <returns>CellInfo of the newly created cell.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if this project is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public CellInfo CreateCell(Stream value, ClientMetadata clientMetadata)
        {
            log.Info("Start CREATE request.");
            return SetCell("", value, clientMetadata);
        }

        /// <summary>Create new cell.</summary>
        /// <param name="json">Json value to set to the cell.</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <returns>CellInfo of the newly created cell.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if this project is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public CellInfo CreateCell(string json, ClientMetadata clientMetadata)
        {
            log.Info("Start CREATE request.");
            return SetCell(null, json, clientMetadata);
        }

        /// <summary>Create new cell (Asyncronous method).</summary>
        /// <param name="value">Value</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <returns>CellInfo of the newly created cell.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if this project is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<CellInfo> CreateCellAsync(Stream value, ClientMetadata clientMetadata)
        {
            log.Info("Start CREATE request.");
            return await SetCellAsync("", value, clientMetadata);
        }

        /// <summary>Create new cell (Asyncronous method).</summary>
        /// <param name="json">Json value to set to the cell.</param>
        /// <param name="clientMetadata">ClientMetadata to associate with the cell.</param>
        /// <returns>CellInfo of the newly created cell.</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if Capability.CLIENT_METADATA is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if this project is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<CellInfo> CreateCellAsync(string json, ClientMetadata clientMetadata)
        {
            log.Info("Start CREATE request.");
            return await SetCellAsync(null, json, clientMetadata);
        }

        #endregion

        #region Delete cell

        /// <summary>Delete cell by id.</summary>
        /// <param name="cellId">Id of the cell to be deleted.</param>
        /// <returns>CellInfo of the deleted cell.</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public CellInfo DeleteCell(string cellId)
        {
            log.Info("Start DELETE request.");
            CellInfo cellInfo = null;

            try
            {
                var deleteUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, deleteUrl, cookies);
                request.Method = "DELETE";

                //attach clientInfo to the request
                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo);

                log.Info("Start getting response.");

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    cellInfo = DataSerializer.Deserialize<CellInfo>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cellInfo;
        }

        /// <summary>Delete cell by id (Asyncronous method).</summary>
        /// <param name="cellId">Id of the cell to be deleted.</param>
        /// <returns>CellInfo of the deleted cell.</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if project/cell is readonly.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<CellInfo> DeleteCellAsync(string cellId)
        {
            log.Info("Start DELETE request.");
            CellInfo cellInfo = null;

            try
            {
                var deleteUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + string.Format(FluxApiData.CellIdUrl, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, deleteUrl, cookies);
                request.Method = "DELETE";

                //attach clientInfo to the request
                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo);

                log.Info("Start getting response.");

                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    cellInfo = DataSerializer.Deserialize<CellInfo>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return cellInfo;
        }

        #endregion

        #region Notifications

        /// <summary>Subscribe to cell events. Note: call only if NOTIFICATION capability is supported.</summary>
        /// <param name="notificationsTypes">Notification type to subscribe for</param>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if NOTIFICATION is not supported.</exception>
        public void Subscribe(NotificationType notificationsTypes)
        {
            log.Info("Subscribing to the notifications {0}.", notificationsTypes.ToString());

            if (!capability.HasFlag(Capability.NOTIFICATION))
                throw new Exceptions.UnsupportedCapabilityException(Capability.NOTIFICATION);

            subscribedNotificationTypes = notificationsTypes;
        }

        /// <summary>Subscribe to cell events. Note: call only if NOTIFICATION capability is supported.</summary>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        private void SubscribeToAllNotifications()
        {
            log.Info("Subscribing to ALL notifications.");

            var subscribeMsg = new SubscribeEvent();
            subscribeMsg.type = "DATATABLE";
            subscribeMsg.body = new SubscribeEvent.EventMessage();
            subscribeMsg.body.Type = "SUBSCRIBE";
            subscribeMsg.body.Data = new SubscribeEvent.EventMessage.MessageData();
            subscribeMsg.body.Data.Types = NotificationType.__ALL__.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var jsonStr = DataSerializer.Serialize(subscribeMsg);
            WebSocket.Send(jsonStr);
        }

        /// <summary>Unsubscribe from datatable notifications.</summary>
        /// <param name="notificationsType">Notification type to unsubscribe from</param>
        public void Unsubscribe(NotificationType notificationsType)
        {
            subscribedNotificationTypes &= ~ notificationsType;
        }

        private void OnErrorMessage(Error error)
        {
            if (OnError != null)
                OnError(this, new ErrorEventArgs(error, projectId));
        }

        private void OnNotificationMessage(Notification notification)
        {
            switch (notification.CellEvent.Type)
            {
                case NotificationType.CELL_CREATED:
                    {
                        Cells.Add(notification.CellInfo);
                    }
                    break;

                case NotificationType.CELL_DELETED:
                    {
                        var deletedCell = cells.FirstOrDefault(x => x == notification.CellInfo);
                        if (deletedCell != null)
                            Cells.Remove(deletedCell);
                    }
                    break;

                case NotificationType.CELL_MODIFIED:
                    {
                        var modifiedCell = cells.FirstOrDefault(x => x == notification.CellInfo);
                        if (modifiedCell != null)
                        {
                            var index = cells.IndexOf(modifiedCell);
                            if (index >= 0)
                                Cells[index] = notification.CellInfo;
                        }
                        else
                            Cells.Add(notification.CellInfo);
                    }
                    break;

                case NotificationType.CELL_CLIENT_METADATA_MODIFIED:
                    {
                        var cellInfo = cells.FirstOrDefault(x => x == notification.CellInfo);
                        if (cellInfo != null)
                            cellInfo.ClientMetadata = notification.CellInfo.ClientMetadata;
                    }
                    break;
            }

            if (capability.HasFlag(Capability.METADATA))
                cells = Cells.OrderByDescending(x => x.Metadata.Modify.GetDate()).ToList();

            if (OnNotification != null && subscribedNotificationTypes.HasFlag(notification.CellEvent.Type))
                OnNotification(this, new NotificationEventArgs(notification, projectId));
        }

        private void WebSocket_OnReconnected()
        {
            var subscribeMsg = new SubscribeEvent();
            subscribeMsg.type = "DATATABLE";
            subscribeMsg.body = new SubscribeEvent.EventMessage();
            subscribeMsg.body.Data = new SubscribeEvent.EventMessage.MessageData();
            subscribeMsg.body.Type = "SUBSCRIBE";
            subscribeMsg.body.Data.Types = NotificationType.__ALL__.ToString().Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            var jsonStr = DataSerializer.Serialize(subscribeMsg);
            WebSocket.Send(jsonStr);

            if (OnWebSocketReconnected != null)
                OnWebSocketReconnected();
        }

        #endregion

        #region Get cell value reference

        /// <summary>Provides a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported.</summary>
        /// <param name="cellId">Id of the cell.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Permanent reference to value</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public CellRefResult GetCellValueReference(string cellId, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var getReturn = new CellRefResult();
            try
            {
                if (!capability.HasFlag(Capability.VALUE_REFERENCE))
                    throw new Exceptions.UnsupportedCapabilityException(Capability.VALUE_REFERENCE);

                log.Info("Start VALUE_REFERENCE request.");
                var getRefUrl = string.Format(FluxApiData.CellUrl + FluxApiData.RefUrl, projectId, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, getRefUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting response.");

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    getReturn.Info = HttpWebClientHelper.ExtractAuxParams<CellInfo>(response);
                    getReturn.ValueReference = DataSerializer.Deserialize<string>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return getReturn;
        }

        /// <summary>Provides a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported.</summary>
        /// <param name="cell">CellInfo</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Permanent reference to value</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public CellRefResult GetCellValueReference(CellInfo cell, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            if (cell == null)
                throw new Exceptions.InternalSDKException("Cell cannot be null.");

            return GetCellValueReference(cell.CellId, extractCellMetadata, extractClientMetadata);
        }

        /// <summary>Provides a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported (Asyncronous method).</summary>
        /// <param name="cellId">Id of the cell.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Permanent reference to value</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public async Task<CellRefResult> GetCellValueReferenceAsync(string cellId, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var getReturn = new CellRefResult();
            try
            {
                if (!capability.HasFlag(Capability.VALUE_REFERENCE))
                    throw new Exceptions.UnsupportedCapabilityException(Capability.VALUE_REFERENCE);

                log.Info("Start VALUE_REFERENCE request.");
                var getRefUrl = string.Format(FluxApiData.CellUrl + FluxApiData.RefUrl, projectId, cellId);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, getRefUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting response.");

                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    getReturn.Info = HttpWebClientHelper.ExtractAuxParams<CellInfo>(response);
                    getReturn.ValueReference = DataSerializer.Deserialize<string>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return getReturn;
        }

        /// <summary>Provides a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported (Asyncronous method).</summary>
        /// <param name="cell">CellInfo of the cell.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>Permanent reference to value</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public async Task<CellRefResult> GetCellValueReferenceAsync(CellInfo cell, bool extractCellMetadata = false,
            bool extractClientMetadata = false)
        {
            if (cell == null)
                throw new Exceptions.InternalSDKException("Cell cannot be null.");

            var getValueRefTask = await GetCellValueReferenceAsync(cell.CellId, extractCellMetadata, extractClientMetadata);
            return getValueRefTask;
        }

        #endregion

        #region Dereference cell value reference

        /// <summary>Dereference a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported.</summary>
        /// <param name="valueRef">Cell reference to release.</param>
        /// <param name="extractValueMetadata">If set to true then metadata associated with the value will be extracted.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns>CellReleaseRefResult</returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public CellReleaseRefResult DereferenceCellValueRef(string valueRef, bool extractValueMetadata = false, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var derefReturn = new CellReleaseRefResult();
            try
            {
                if (!capability.HasFlag(Capability.VALUE_REFERENCE))
                    throw new Exceptions.UnsupportedCapabilityException(Capability.VALUE_REFERENCE);

                log.Info("Start DEREF_VALUE_REFERENCE response.");
                var derefUrl = string.Format(FluxApiData.ValueRefUrl, valueRef);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, derefUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractValueMetadata)
                    optParams["ValueMetadata"] = extractValueMetadata;
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting repsonse.");
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    derefReturn = HttpWebClientHelper.ExtractAuxParams<CellReleaseRefResult>(response);
                    derefReturn.Value = DataSerializer.Deserialize<string>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return derefReturn;
        }

        /// <summary>Dereference a permanent reference to value. Note: call only if VALUE_REFERENCE capability is supported (Asyncronous method).</summary>
        /// <param name="valueRef">Cell reference to release.</param>
        /// <param name="extractValueMetadata">If set to true then metadata associated with the value will be extracted.</param>
        /// <param name="extractCellMetadata">If set to true then metadata associated with this cell will be extracted.</param>
        /// <param name="extractClientMetadata">If set to true then client metadata associated with this cell will be extracted.</param>
        /// <returns></returns>
        /// <exception cref="Exceptions.UnsupportedCapabilityException">Throws if VALUE_REFERENCE is not supported.</exception>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception> 
        public async Task<CellReleaseRefResult> DereferenceCellValueRefAsync(string valueRef, bool extractValueMetadata = false, bool extractCellMetadata = false, bool extractClientMetadata = false)
        {
            var derefReturn = new CellReleaseRefResult();
            try
            {
                if (!capability.HasFlag(Capability.VALUE_REFERENCE))
                    throw new Exceptions.UnsupportedCapabilityException(Capability.VALUE_REFERENCE);

                log.Info("Start DEREF_VALUE_REFERENCE response.");
                var derefUrl = string.Format(FluxApiData.ValueRefUrl, valueRef);
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, derefUrl, cookies);
                request.Method = "GET";

                var optParams = new Dictionary<string, object>();
                if (extractValueMetadata)
                    optParams["ValueMetadata"] = extractValueMetadata;
                if (extractClientMetadata)
                {
                    log.Info("Adding extractClientMetadata optional parameter.");

                    if (capability.HasFlag(Capability.CLIENT_METADATA))
                        optParams["ClientMetadata"] = extractClientMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.CLIENT_METADATA);
                }
                if (extractCellMetadata)
                {
                    log.Info("Adding extractCellMetadata optional parameter.");

                    if (capability.HasFlag(Capability.METADATA))
                        optParams["Metadata"] = extractCellMetadata;
                    else
                        throw new Exceptions.UnsupportedCapabilityException(Capability.METADATA);
                }

                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo, optParams);
                log.Info("Start getting repsonse.");
                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    derefReturn = HttpWebClientHelper.ExtractAuxParams<CellReleaseRefResult>(response);
                    derefReturn.Value = DataSerializer.Deserialize<string>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return derefReturn;
        }

        #endregion

        #region Update Cell List

        /// <summary>Get Cells for current project (Asyncronous method).</summary>
        /// <returns>List of CellInfo</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task UpdateCellsAsync()
        {
            log.Info("Trying to get cells.");

            try
            {
                var cellUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + FluxApiData.CellBaseUrl;
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, cellUrl, cookies);
                request.Method = "GET";

                //attach clientInfo to the request
                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo);

                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    cells = DataSerializer.Deserialize<List<CellInfo>>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        /// <summary>Get Cells for current project.</summary>
        /// <returns>List of CellInfo</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public void UpdateCells()
        {
            log.Info("Trying to get cells.");

            try
            {
                var cellUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + FluxApiData.CellBaseUrl;
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, cellUrl, cookies);
                request.Method = "GET";

                //attach clientInfo to the request
                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo);

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    cells = DataSerializer.Deserialize<List<CellInfo>>(StreamUtils.GetDecompressedResponseStream(response));
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }
        }

        #endregion

        /// <summary>Get capabilities supported for current project.</summary>
        /// <returns>Capability</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        private Capability GetCapabilities()
        {
            Capability capabilities = Capability.NONE;
            try
            {
                log.Info("Trying to get capabilities.");
                var capabilitiesUrl = string.Format(FluxApiData.DataTableAPIUrl, projectId) + FluxApiData.CapabilityUrl;
                var request = HttpWebClientHelper.CreateRequest(sdkMetadata, capabilitiesUrl, cookies);

                //attach clientInfo to the request
                HttpWebClientHelper.UpdateRequestWithOpt(request, sdkMetadata.ClientInfo);

                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    var capabilityArr = DataSerializer.Deserialize<string[]>(StreamUtils.GetDecompressedResponseStream(response));
                    foreach (var value in capabilityArr)
                    {
                        Capability res;
                        if (Enum.TryParse<Capability>(value, out res))
                            capabilities = capabilities | res;
                    }
                }
            }
            catch (Exceptions.FluxException ex)
            {
                log.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new Exceptions.InternalSDKException(ex.Message);
            }

            return capabilities;
        }

        //Json Subscribe object structures
        #region Subscription request models

        private struct SubscribeEvent
        {
            public string type;
            public EventMessage body;

            public struct EventMessage
            {
                public string Type;
                public MessageData Data;

                public struct MessageData
                {
                    public string[] Types;
                }
            }
        }

        #endregion

        private void ProgressStream_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            if (ProgressTracker != null)
                ProgressTracker.Report(e.Position, e.Length);
        }
    }

    /// <summary>Contains available datatable capabilities</summary>
    [Flags]
    public enum Capability
    {
        /// <summary>Datatable capability not set.</summary>
        NONE = 1,
        /// <summary>Cell metadata is available.</summary>
        METADATA = 2,
        /// <summary>Cell client metadata is available.</summary>
        CLIENT_METADATA = 4,
        /// <summary>Cell notifications are available.</summary>
        NOTIFICATION = 8,
        /// <summary>Access to cell value using reference is available.</summary>
        VALUE_REFERENCE = 16,
        /// <summary>Cell values history is available</summary>
        HISTORY = 32
    }

    /// <summary>Contains available notification types</summary>
    [Flags]
    public enum NotificationType
    {
        /// <summary>Notification type not set.</summary>
        __NONE__ = 0,
        /// <summary>Cell modified notification.</summary>
        CELL_MODIFIED = 1,
        /// <summary>Cell created notification.</summary>
        CELL_CREATED = 2,
        /// <summary>Cell deleted notification.</summary>
        CELL_DELETED = 4,
        /// <summary>Cell metadata modified notification.</summary>
        CELL_CLIENT_METADATA_MODIFIED = 8,
        /// <summary>All notification listed above are set.</summary>
        __ALL__ = ~__NONE__
    }
}