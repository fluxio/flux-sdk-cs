using System;
using System.Collections.Generic;
using System.Linq;
using Flux.SDK.Types;
using Flux.ConsoleApp.Helpers;

namespace Flux.ConsoleApp.Commands
{
    public static partial class Command
    {
        public static void ProjectList()
        {
            CheckForInitAndLoginSDK();

            List<Project> projects = SDK.CurrentUser.Projects;
            for (int i = 0; i < projects.Count; i++)
                Console.WriteLine("Project #{0}: id = {1}, name = '{2}'", i, projects[i].Id, projects[i].Name);
        }

        public static void CreateProject(string projectName)
        {
            CheckForInitAndLoginSDK();

            Project project = SDK.CurrentUser.CreateNewProject(projectName);
            Console.WriteLine("Created project id = '{0}', name = '{1}'", project.Id, project.Name);
        }

        public static void DeleteProject(string projectId)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            string keyPromt = project.DataTable.Cells.Any()
                ? string.Format("with keys : '{0}'", string.Join(", ", project.DataTable.Cells.Select(c => c.ClientMetadata.Label)))
                : "witout keys";

            string prompt =
                string.Format(
                    "You are going to delete project: '{0}'{2}{1},{2}Please note that this action cannot be undone. you will lose all your data associated with this project. Press 'Y' to confirm."
                    , project.Name
                    , keyPromt
                    , Environment.NewLine);

            if (PromptManager.ShowPrompt(prompt))
            {
                if (SDK.CurrentUser.DeleteProject(projectId))
                    Console.WriteLine("Project '{0}' has been deleted", projectId);
                else
                    Console.WriteLine("Project '{0}' was not deleted", projectId);
            }
        }

        public static void ProjectHelp()
        {
            string help =
@"Helping work with projects (list, create, delete) are available for the currently logged in user.

PROJECT LIST
PROJECT DELETE {id Project}
PROJECT CREATE {name Project}

For example: 
    PROJECT LIST
    PROJECT DELETE 'projectId'
    PROJECT CREATE ""My first project""";

            Console.WriteLine(help);
        }
    }
}
