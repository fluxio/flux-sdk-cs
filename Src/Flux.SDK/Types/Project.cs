using Flux.Logger;
using Flux.SDK.Properties;
using Flux.SDK.DataTableAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Flux.SDK.WebServices;
using System.Net;
using Flux.Serialization;
using System.Runtime.Serialization;

namespace Flux.SDK.Types
{
    /// <summary>Represents the user project</summary>
    [Serializable]
    [DataContract]
    public class Project
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|Project");

        #region Serializable fields

        /// <summary>The project id</summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>The project name</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>The project creator</summary>
        [DataMember(Name = "creator")]
        public string Creator { get; set; }

        /// <summary>The project last update date</summary>
       [DataMember(Name = "last_updated")]
        public DateTime UpdatedDate { get; set; }

        /// <summary>The project creation date</summary>
        [DataMember(Name = "created_at")]
        public DateTime CreatedDate { get; set; }

        /// <summary>The user role on the project</summary>
        [DataMember(Name = "acl")]
        public UserRoleType UserRole { get; set; }

        /// <summary>The project kind</summary>
        [DataMember(Name = "kind")]
        public ProjectKind Kind { get; set; }

        #endregion

        /// <summary>Gets whether project is readonly</summary>
        public bool IsReadOnly
        {
            get
            {
                return UserRole == UserRoleType.Viewer || UserRole == UserRoleType.None || Kind == ProjectKind.Readonly ||
                       Kind == ProjectKind.None;
            }
        }

        internal SDKMetadata SdkMetadata { get; set; }

        [NonSerialized()]
        private List<FluxCookie> cookies;
        internal List<FluxCookie> Cookies
        {
            get
            {
                return cookies;
            }
            set
            {
                cookies = value;

                if (dataTable != null)
                    dataTable.Cookies = value;
            }
        }

        [NonSerialized()]
        private DataTable dataTable;
        /// <summary>Returns datatable for the project</summary>
        public DataTable DataTable
        {
            get
            {
                if (dataTable == null)
                    dataTable = new DataTable(Id, Cookies, SdkMetadata);

                return dataTable;
            }
        }

        /// <summary>Convert brep to specified format.</summary>
        /// <param name="content">Brep to convert (base64 encoded string).</param>
        /// <param name="sourceFormat">Source format of brep.</param>
        /// <param name="targetFormat">Target format of brep</param>
        /// <returns>Converted brep (base64 encoded string).</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public string ConvertBrep(string content, string sourceFormat, string targetFormat)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            #region Create object wrapper for brep

            var brep = new BrepToConvert.SceneObject.BrepPrimitive();
            brep.Primitive = "brep";
            brep.Format = sourceFormat;
            brep.Content = content;

            var entities = new BrepToConvert.SceneObject.EntitiesObject();
            entities.Brep = brep;

            BrepToConvert.SceneObject scene = new BrepToConvert.SceneObject();
            scene.Entities = entities;
            var initObj = new BrepToConvert.SceneObject.OperationObj();
            initObj.Name = "result";
            initObj.Op = new string[] { "repr", targetFormat, "brep@1" };

            scene.Operations = new object[] { initObj };

            var brepToConvert = new BrepToConvert();
            brepToConvert.Scene = scene;

            #endregion

            var request = WriteBrepToRequest(brepToConvert);

            try
            {
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    dynamic responseJson = DataSerializer.DynamicDeserialize(StreamUtils.GetDecompressedResponseStream(response));
                    dynamic convertedContent = responseJson.Output.Results.value.result.content;
                    if (convertedContent != null)
                    {
                        object contentStr = convertedContent;
                        return contentStr.ToString();
                    }
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

            return null;
        }

        /// <summary>
        /// Tessellates brep.
        /// </summary>
        /// <param name="content">Brep to tesselate (base64 encoded string).</param>
        /// <param name="sourceFormat">Source format of brep.</param>
        /// <param name="lod">Level of detail of the brep.</param>
        /// <param name="unit">Units of the brep.</param>
        /// <returns>Tessellated brep (base64 encoded string).</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public string TessellateBrep(string content, string sourceFormat, object lod, object unit)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            #region Create object wrapper for brep

            var brep = new BrepToConvert.SceneObject.BrepPrimitive();
            brep.Primitive = "brep";
            brep.Format = sourceFormat;
            brep.Content = content;

            var entities = new BrepToConvert.SceneObject.EntitiesObject();
            entities.Brep = brep;

            var scene = new BrepToConvert.SceneObject();
            scene.Entities = entities;
            var initObj = new BrepToConvert.SceneObject.OperationObj();
            initObj.Name = "result";
            initObj.Op = new object[] { "tessellateJson", "brep@1", lod, unit };

            scene.Operations = new object[] { initObj };

            var brepToTessellate = new BrepToConvert();
            brepToTessellate.Scene = scene;

            #endregion

            var request = WriteBrepToRequest(brepToTessellate);

            try
            {
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    dynamic responseJson = DataSerializer.DynamicDeserialize(StreamUtils.GetDecompressedResponseStream(response));
                    dynamic tesselatedContent = responseJson.Output.Results.value.result;
                    if (tesselatedContent != null)
                        return tesselatedContent.ToString();
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

            return null;
        }

        private HttpWebRequest WriteBrepToRequest(BrepToConvert brep)
        {
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, string.Format(FluxApiData.ConvertUrl, Id), Cookies);
            request.Method = "POST";

            var jsonStr = DataSerializer.Serialize(brep);

            byte[] bytes = Encoding.UTF8.GetBytes(jsonStr);
            request.ContentLength = bytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            return request;
        }

        #region Auxiliary objects for Brep conversion

        [DataContract]
        internal class BrepToConvert
        {
            [DataMember(Name ="Scene")]
            public SceneObject Scene { get; set; }

            internal class SceneObject
            {
                [DataMember(Name = "Entities")]
                public EntitiesObject Entities { get; set; }

                [DataMember(Name = "Operations")]
                public object[] Operations { get; set; }

                [DataContract]
                internal class EntitiesObject
                {
                   [DataMember(Name = "brep@1")]
                    public object Brep;
                }

                [DataContract]
                internal class BrepPrimitive
                {
                    [DataMember(Name = "primitive")]
                    public string Primitive;
                    [DataMember(Name = "format")]
                    public string Format;
                    [DataMember(Name = "content")]
                    public string Content;
                }

                [DataContract]
                internal class OperationObj
                {
                    [DataMember(Name = "name")]
                    public string Name { get; set; }
                    [DataMember(Name = "op")]
                    public object[] Op { get; set; }
                }
            }
        }

        #endregion

        /// <summary>Represents the kind of the project</summary>
        public enum ProjectKind
        {
            /// <summary>Project kind not set</summary>
            None = 0,
            /// <summary>Represents full access to project</summary>
            Full,
            /// <summary>Represents readonly access to project</summary>
            Readonly
        }

        /// <summary>Represents the user role on the project</summary>
        public enum UserRoleType
        {
            /// <summary>User role not set.</summary>
            None = 0,
            /// <summary>Set if user is owner for project</summary>
            Owner,
            /// <summary>Set if user is collaborator for project</summary>
            Collaborator,
            /// <summary>Set if user is viewer for project</summary>
            Viewer
        }
    }
}