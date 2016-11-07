using System;

namespace Flux.ConsoleApp.Commands
{
    static partial class Command
    {
        private static void UserLogout()
        {
            Console.WriteLine("User has been successfully logged out");
        }

        public static void Logout()
        {
            CheckForInitAndLoginSDK();

            SDK.OnUserLogin -= UserLogin;
            SDK.OnUserLogout += UserLogout;

            SDK.Logout();

            SDK.OnUserLogout -= UserLogout;
        }

        public static void LogoutHelp()
        {
            string help =
@"Logout the current user from sdk, clears saved cookies.

LOGOUT [no parameter]

For example: 
    LOGOUT";

            Console.WriteLine(help);
        }
    }
}
