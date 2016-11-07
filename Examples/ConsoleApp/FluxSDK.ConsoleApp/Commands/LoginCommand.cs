using System;
using Flux.SDK.Types;

namespace Flux.ConsoleApp.Commands
{
    public static partial class Command
    {
        private static void UserLogin(User user)
        {
            //save current position
            int top = Console.CursorTop;
            int left = Console.CursorLeft;

            //move promt command to bottom
            Console.MoveBufferArea(0, Console.CursorTop, Console.WindowWidth, 1, 0, Console.CursorTop + 1);

            //show message
            Console.SetCursorPosition(0, top);
            Console.WriteLine("User has been logged in as: {0}", user.FullName);

            //return to saved position
            Console.SetCursorPosition(left, top + 1);
        }

        public static void Login(string clientSecret, string serverUrl)
        {
            if (SDK == null)
                throw new Exception("You must initialize sdk before this command");

            SDK.OnUserLogin += UserLogin;

            SDK.Login(clientSecret, serverUrl);
        }

        public static void LoginHelp()
        {
            string help =
@"Login to Flux using specified client secret and server url

LOGIN {client secret} {server url}

For example: 
    LOGIN """" https://flux.io";
            Console.WriteLine(help);
        }
    }
}
