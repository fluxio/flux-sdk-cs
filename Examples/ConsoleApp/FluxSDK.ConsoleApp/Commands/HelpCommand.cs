using System;

namespace Flux.ConsoleApp.Commands
{
    public static partial class Command
    {
        public static void Help()
        {
            string help =
@"HELP            Help command
INIT            Init sdk using specified ClientID, ClientVersion and
                AdditionalData (optional)
PROJECT         Helping work with projects (list, create, delete) are available
                for the currently logged in user.
KEY             Helping work with keys (list, create, delete, get, set) are
                available for the currently logged in user.
LOGIN           Login to Flux using specified client secret and server url
LOGOUT          Logout the current user from sdk, clears saved cookies.
EXIT            Close FluxSDK Console application
";
            Console.WriteLine(help);
        }
    }
}
