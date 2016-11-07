using System;
using Flux.SDK;

namespace Flux.ConsoleApp.Commands
{
    static partial class Command
    {
        public static void Init(string clientId, string clientVersion)
        {
            SDK = new FluxSDK(clientId, clientVersion);

            Console.WriteLine("Sdk has been successfully initialized.");

            if (SDK.CurrentUser != null)
                Console.WriteLine("User has been logged in as: {0}", SDK.CurrentUser.FullName); 
        }

        public static void InitHelp()
        {
            string help =
@"Init sdk using specified ClientID, ClientVersion

INIT {client id} {client version}

For example: 
    INIT XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX dev
";
            Console.WriteLine(help);
        }
    }
}
