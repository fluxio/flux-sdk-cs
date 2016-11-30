using Flux.Logger;
using Flux.SDK.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Flux.SDK.WebServices;
using System.Threading.Tasks;
using Flux.Serialization;
using System.Runtime.Serialization;

namespace Flux.SDK.Types
{
    /// <summary>Represents the Flux user</summary>
    [DataContract]
    public class User
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|FluxUser");

        #region Serializable fields

        /// <summary>The user id</summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>The user first name </summary>
        [DataMember(Name = "first_name")]
        public string FirstName { get; set; }

        /// <summary>The user last name</summary>
        [DataMember(Name = "last_name")]
        public string LastName { get; set; }

        /// <summary>The user email</summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>The user kind</summary>
        [DataMember(Name = "kind")]
        public string Kind { get; set; }

        #endregion     

        internal SDKMetadata SdkMetadata { get; set; }

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
                if (cookies == null && projects != null)
                {
                    foreach (var p in projects)
                        p.Cookies = null;

                    return;
                }

                UpdateProjects();
            }
        }

        /// <summary>The user full name</summary>
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        private List<Project> projects;
		/// <summary>The user's projects</summary>
        public List<Project> Projects
        {
            get
            {
                return projects ?? new List<Project>();
            }
            set
            {
                if (projects != null && value != null)
                {
                    var updatedProjectsCache = projects.Where(p1 => value.Any(p2 => p2.Id == p1.Id)).ToList();
                    updatedProjectsCache.ForEach(x =>
                    {
                        var p = value.First(y => y.Id == x.Id);
                        x.Name = p.Name;
                        x.UpdatedDate = p.UpdatedDate;
                    });
                    updatedProjectsCache.AddRange(value.Where(p2 => updatedProjectsCache.All(p1 => p1.Id != p2.Id)));
                    projects = updatedProjectsCache;
                }
                else
                    projects = value;

                if (projects != null)
                {
                    foreach (var project in projects)
                    {
                        project.SdkMetadata = SdkMetadata;
                        project.Cookies = Cookies;
                    }
                }
            }
        }

        #region Create new project

        /// <summary>Create new project.</summary>
        /// <param name="projectName">name of the new project.</param>
        /// <returns>Project instance if project was created successfully.</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public Project CreateNewProject(string projectName)
        {
            Project newProject = null;
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, FluxApiData.ProjectsUrl, Cookies);
            request.Method = "POST";

            try
            {
                string formParams = string.Format("user={0}&name={1}&app={2}", Uri.EscapeDataString(Id), Uri.EscapeDataString(projectName), Uri.EscapeDataString("blank"));
                byte[] bytes = Encoding.ASCII.GetBytes(formParams);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    newProject = DataSerializer.Deserialize<Project>(StreamUtils.GetDecompressedResponseStream(response));
                    if (newProject != null)
                    {
                        newProject.Cookies = Cookies;
                        newProject.SdkMetadata = SdkMetadata;
                    }

                    if (projects != null)
                        projects.Add(newProject);
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

            return newProject;
        }

        /// <summary>Create new project.</summary>
        /// <param name="projectName">name of the new project.</param>
        /// <returns>Porject instance if project was created successully.</returns>
        /// <exception cref="Exceptions.FluxException">Throws for internal SDK exceptions (Network is down, etc.).</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<Project> CreateNewProjectAsync(string projectName)
        {
            Project newProject = null;
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, FluxApiData.ProjectsUrl, Cookies);
            request.Method = "POST";

            try
            {
                string formParams = string.Format("user={0}&name={1}&app={2}", Uri.EscapeDataString(Id), Uri.EscapeDataString(projectName), Uri.EscapeDataString("blank"));
                byte[] bytes = Encoding.ASCII.GetBytes(formParams);
                request.ContentLength = bytes.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    newProject = DataSerializer.Deserialize<Project>(StreamUtils.GetDecompressedResponseStream(response));
                    if (newProject != null)
                    {
                        newProject.Cookies = Cookies;
                        newProject.SdkMetadata = SdkMetadata;
                    }

                    if (projects != null)
                        projects.Add(newProject);
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

            return newProject;
        }

        #endregion

        #region Delete project

        /// <summary>Delete project by id.</summary>
        /// <param name="projectId">Id of the project to be deleted.</param>
        /// <returns>true if project was deleted successfully.</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if user doesn't have rights to delete this project.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public bool DeleteProject(string projectId)
        {
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, string.Format(FluxApiData.ProjectDeleteUrl, projectId), Cookies);
            request.Method = "DELETE";
            try
            {
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    if (response != null)
                    {
                        var project = Projects.FirstOrDefault(x => x.Id == projectId);
                        if (projects != null && project != null)
						{
							project.Cookies = null;
							projects.Remove(project);
						}

                        return true;
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

            return false;
        }

        /// <summary>Delete project by id.</summary>
        /// <param name="projectId">Id of the project to be deleted.</param>
        /// <returns>true if project was deleted successfully.</returns>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ForbiddenException">Throws if user doesn't have rights to delete this project.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task<bool> DeleteProjectAsync(string projectId)
        {
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, string.Format(FluxApiData.ProjectDeleteUrl, projectId), Cookies);
            request.Method = "DELETE";
            try
            {
                using (var response = await HttpWebClientHelper.GetResponseAsync(request))
                {
                    if (response != null)
                    {
                        var project = Projects.FirstOrDefault(x => x.Id == projectId);
                        if (projects != null && project != null)
						{
							project.Cookies = null;
							projects.Remove(project);
						}

                        return true;
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

            return false;
        }

        #endregion

        #region Update list of available projects

        /// <summary>Update list of projects for the current user. </summary>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public async Task UpdateProjectsAsync()
        {
            var projectList = new List<Project>();
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, FluxApiData.ProjectsUrl, Cookies);
            request.Method = "GET";
            request.Headers.Add("user", Id);
            try
            {
                using (var response = await HttpWebClientHelper.GetResponseAsync(request).ConfigureAwait(false))
                {
                    projectList = DataSerializer.Deserialize<List<Project>>(StreamUtils.GetDecompressedResponseStream(response));
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

            Projects = projectList;
        }

        /// <summary>Update list of projects for the current user. </summary>
        /// <exception cref="Exceptions.ConnectionFailureException">Throws if network connection is down.</exception>
        /// <exception cref="Exceptions.UnathorizedException">Throws if provided cookies were obsolete.</exception>
        /// <exception cref="Exceptions.ServerUnavailableException">Throws if Flux server is down.</exception>
        /// <exception cref="Exceptions.InternalSDKException">Throws for unhandled SDK exceptions.</exception>
        public void UpdateProjects()
        {
            var projectList = new List<Project>();
            var request = HttpWebClientHelper.CreateRequest(SdkMetadata, FluxApiData.ProjectsUrl, Cookies);
            request.Method = "GET";
            request.Headers.Add("user", Id);
            try
            {
                using (var response = HttpWebClientHelper.GetResponse(request))
                {
                    projectList = DataSerializer.Deserialize<List<Project>>(StreamUtils.GetDecompressedResponseStream(response));
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

            Projects = projectList;
        }

        #endregion

        internal static User GetWhoAmI(IEnumerable<FluxCookie> cookies, SDKMetadata sdkMetadata)
        {
            if (cookies == null)
                return null;

            var authCookie = cookies.FirstOrDefault(el => el.CookieName == FluxApiData.CookiesAuthName);
            if (authCookie == null)
                return null;

            User user = null;
            var request = HttpWebClientHelper.CreateRequest(sdkMetadata, FluxApiData.WhoAmIUrl, cookies.ToList());
            request.Method = "GET";
            using (var response = HttpWebClientHelper.GetResponse(request))
            {
                user = DataSerializer.Deserialize<User>(StreamUtils.GetDecompressedResponseStream(response));
            }

            user.SdkMetadata = sdkMetadata;
            user.Cookies = cookies.ToList();

            return user;
        }
    }
}