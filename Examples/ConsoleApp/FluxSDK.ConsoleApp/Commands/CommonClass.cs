using System;
using System.Linq;
using Flux.SDK;
using Flux.SDK.Types;

namespace Flux.ConsoleApp.Commands
{
    public static partial class Command
    {
        private static FluxSDK SDK { get; set; }

        private static void CheckForInitAndLoginSDK()
        {
            //checking init SDK
            if (SDK == null)
                throw new Exception("You must initialize sdk before this command.");

            //checking init user
            if (SDK.CurrentUser == null)
                throw new Exception("You must log in before use this command.");
        }

        private static Project GetProjectById(string projectId)
        {
            Project project = SDK.CurrentUser.Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                throw new Exception(string.Format("Project with specified id: {0} doesn't exit.{1}Please specify valid id (see list of projects for details).", projectId, Environment.NewLine));
            return project;
        }
    }
}
