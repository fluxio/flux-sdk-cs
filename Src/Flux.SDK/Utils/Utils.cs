using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace Flux.SDK
{
    internal static class Utils
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK");
        private static Uri BaseUri;        

        internal static string EncodeHeader(Flux.SDK.WebServices.OptHeader header)
        {            
            var jsonStr = header.ToJson();            
            var jsonStrBytes = Encoding.UTF8.GetBytes(jsonStr);
            return System.Convert.ToBase64String(jsonStrBytes);
        }

        internal static string DecodeHeader(string header) 
        {
            var base64EncodedBytes = System.Convert.FromBase64String(header);
            var jsonStr = Encoding.UTF8.GetString(base64EncodedBytes);
            return jsonStr;
        }

        internal static void RemoveOICCookies(string path, string clientId)
        {
            FileStream stream = null;

            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var objToSerialize = new List<object>();

                if (File.Exists(path))
                {
                    stream = File.OpenRead(path);
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Binder = new DeserializationBinder();
                    if (stream.Length > 0)
                    {
                        var deserializedList = (IEnumerable<object>)binaryFormatter.Deserialize(stream);

                        string currentId = string.Empty;
                        var dictionary = new Dictionary<string, List<FluxCookie>>();
                        foreach (var item in deserializedList)
                        {
                            if (item is string)
                            {
                                currentId = (string)item;
                                dictionary[currentId] = new List<FluxCookie>();
                            }
                            else
                            {
                                var cookie = item as FluxCookie;
                                if (cookie != null && !string.IsNullOrEmpty(currentId))
                                    dictionary[currentId].Add(cookie);
                            }
                        }

                        foreach (var kvp in dictionary)
                        {
                            if (kvp.Key != clientId)
                            {
                                objToSerialize.Add(kvp.Key);
                                objToSerialize.AddRange(kvp.Value);
                            }
                        }
                    }

                    stream.Close();

                    if (objToSerialize.Count == 0)
                    {
                        File.Delete(path);
                        return;
                    }

                    if (binaryFormatter != null)
                    {
                        stream = File.OpenWrite(path);
                        binaryFormatter.Serialize(stream, objToSerialize.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn("Unable to remove credentials. See exception for more details");
                log.Warn(ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        internal static void StoreOICCookies(string path, List<FluxCookie> cookies, string clientId)
        {
            FileStream stream = null;

            try
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                List<object> objToSerialize = new List<object>();
                if (cookies != null)
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Binder = new DeserializationBinder();

                    var dictionary = new Dictionary<string, List<FluxCookie>>();

                    if (File.Exists(path))
                    {
                        stream = File.OpenRead(path);
                        if (stream.Length > 0)
                        {
                            var deserializedList = (IEnumerable<object>)binaryFormatter.Deserialize(stream);

                            string currentId = string.Empty;
                            foreach (var item in deserializedList)
                            {
                                if (item is string)
                                {
                                    currentId = (string)item;
                                    dictionary[currentId] = new List<FluxCookie>();
                                }
                                else
                                {
                                    var cookie = item as FluxCookie;
                                    if (cookie != null && !string.IsNullOrEmpty(currentId))
                                        dictionary[currentId].Add(cookie);
                                }
                            }
                        }

                        stream.Close();
                        stream = File.OpenWrite(path);
                    }
                    else
                        stream = File.Create(path);

                    dictionary[clientId] = cookies;

                    foreach (var kvp in dictionary)
                    {
                        objToSerialize.Add(kvp.Key);
                        objToSerialize.AddRange(kvp.Value);
                    }

                    binaryFormatter.Serialize(stream, objToSerialize.ToArray());
                }
            }
            catch (Exception e)
            {
                log.Warn("Unable to store credentials. See exception for more details");
                log.Warn(e);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        internal static IEnumerable<FluxCookie> LoadOICCookies(string path, string clientId)
        {
            if (!File.Exists(path))
            {
                log.Debug("Flux credentials not found.");
                return null;
            }

            FileStream stream = null;

            try
            {
                stream = File.OpenRead(path);
                var formatter = new BinaryFormatter();
                formatter.Binder = new DeserializationBinder();
                var deserializedList = (IEnumerable<object>)formatter.Deserialize(stream);

                string currentId = string.Empty;
                var dictionary = new Dictionary<string, List<FluxCookie>>();
                foreach (var item in deserializedList)
                {
                    if (item is string)
                    {
                        currentId = (string)item;
                        dictionary[currentId] = new List<FluxCookie>();
                    }
                    else
                    {
                        var cookie = item as FluxCookie;
                        if (cookie != null && !string.IsNullOrEmpty(currentId))
                            dictionary[currentId].Add(cookie);
                    }
                }

                log.Info("Credentials successfully loaded.");

                if (dictionary.ContainsKey(clientId))
                    return dictionary[clientId];

                return null;
            }
            catch (Exception e)
            {
                log.Warn("Credentials are found but can't be processed. See exception for more details");
                log.Warn(e);
                return null;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        internal static Uri GetServerUri()
        {
            log.Info("Getting settings Flux host server address...");

            var path = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), Resources.HostsFileName);
            log.Debug("Reading hosts file ({0})", path);

            BaseUri = new Uri(FluxApiData.DefaultServerUrl);

            try
            {
                if (File.Exists(path))
                {
                    log.Debug("Hosts file found. Reading information...");
                    XmlDocument document = new XmlDocument();
                    document.Load(path);
                    XmlNode activeServerURL = document.GetElementsByTagName(Resources.HostsActiveServerURL)[0];
                    int index = Int32.Parse(activeServerURL.InnerText);
                    XmlNodeList allServers = document.GetElementsByTagName(Resources.HostsServerListURL);
                    log.Debug("File read successfull. Readed {0} hosts records. Active node is {1}", allServers.Count, index);
                    string address = allServers[index].InnerText;
                    log.Info("Active server is {0}", address);
                    BaseUri = new Uri(address);
                    return BaseUri;
                }
                else
                {
                    log.Warn("Host file not found. Trying to get info from FluxHost variable");
                    var host = Environment.GetEnvironmentVariable("FluxHost");
                    if (!string.IsNullOrEmpty(host))
                    {
                        log.Debug("Variable found. Active Server is {0}", host);
                        BaseUri = new Uri(host);
                        return BaseUri;
                    }
                    log.Warn("Environment variable not found. Default server value is used ({0})", BaseUri);
                    log.Info("Active server is {0}", BaseUri);
                    return BaseUri;
                }
            }
            catch (Exception)
            {
                log.Warn("Unable to read log file. Trying to get info from FluxHost variable");
                var host = Environment.GetEnvironmentVariable("FluxHost");
                if (!string.IsNullOrEmpty(host))
                {
                    log.Debug("Variable found. Active Server is {0}", host);
                    BaseUri = new Uri(host);
                    return BaseUri;
                }

                log.Warn("Environment variable not found. Default server value is used ({0})", BaseUri);
                log.Info("Active server is {0}", BaseUri);
                return BaseUri;
            }
        }

        internal static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }

                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                    stream.Position = originalPosition;
            }
        }
    }
}
