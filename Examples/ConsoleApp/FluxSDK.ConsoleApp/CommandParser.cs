using System;
using Flux.ConsoleApp.Enums;

namespace Flux.ConsoleApp
{
    public static class CommandParser
    {
        public static CommandType ParseArguments(ref string[] inputArgs)
        {
            //trying parse argunebts
            string command = inputArgs[0].ToUpper();
#region INIT
            // INIT clientID ClientVersion
            if (command == "INIT")
            {
                if (inputArgs.Length < 2)
                    throw new Exception("ClienID not found");

                if (inputArgs.Length < 3)
                    throw new Exception("ClientVersion not found");

                return CommandType.Init;
            }
#endregion

#region LOGIN
            // LOGIN {client secret} {server url}
            if (command == "LOGIN")
            {
                if (inputArgs.Length < 2)
                    throw new Exception("Client secret not found");

                if (inputArgs.Length < 3)
                    throw new Exception("Server url not found");

                return CommandType.Login;
            }
#endregion

#region PROJECT
            // PROJECT LIST|CREATE|DELETE
            if (command == "PROJECT")
            {
                if (inputArgs.Length < 2)
                    throw new Exception("Parameter for command PROJECT not found");

                string commandForProject = inputArgs[1].ToUpper();

                if (commandForProject == "LIST")
                    return CommandType.ProjectList;

                if (commandForProject == "CREATE")
                {
                    if (inputArgs.Length < 3)
                        throw new Exception("Name project not found");

                    return CommandType.ProjectCreate;
                }

                if (commandForProject == "DELETE")
                {
                    if (inputArgs.Length < 3)
                        throw new Exception("Project ID not found");

                    return CommandType.ProjectDelete;
                }

            }
#endregion

#region KEY
            // KEY LIST|CREATE|DELETE|GET|SET
            if (command == "PROJECT" )
            {
                if (inputArgs.Length < 3)
                    throw new Exception("Parameter for command PROJEECT not found");

                command = inputArgs[2].ToUpper();

                if (command == "KEY")
                {
                    if (inputArgs.Length < 4)
                        throw new Exception("Parameter for command KEY not found");

                    string commandForKey = inputArgs[3].ToUpper();

                    if (commandForKey == "LIST")
                        return CommandType.KeyList;

                    if (commandForKey == "CREATE")
                    {
                        if (inputArgs.Length < 5)
                            throw new Exception("Label key not found");

                        if (inputArgs.Length < 6)
                            throw new Exception("Value for create command not found");

                        if (inputArgs.Length < 7)
                        {
                            //made full number of parameters for unused parameter as description
                            Array.Resize(ref inputArgs, inputArgs.Length + 1);

                            inputArgs[6] = inputArgs[5];
                            inputArgs[5] = "";
                        }

                        return CommandType.KeyCreate;
                    }

                    if (commandForKey == "DELETE")
                    {
                        if (inputArgs.Length < 5)
                            throw new Exception("key ID not found");

                        return CommandType.KeyDelete;
                    }

                    if (inputArgs.Length < 5)
                        throw new Exception("Parameter for command KEY not found");

                    string subCommandSecond = inputArgs[4].ToUpper();
                    if (subCommandSecond == "GET")
                    {
                        if (inputArgs.Length < 6)
                        {
                            //made full number of parameters for unused parameter as description
                            Array.Resize(ref inputArgs, inputArgs.Length + 1);
                            inputArgs[5] = "";
                        }

                        return CommandType.KeyGet;
                    }

                    if (subCommandSecond == "SET")
                    {
                        if (inputArgs.Length < 6)
                        {
                            //made full number of parameters for unused parameter as description
                            Array.Resize(ref inputArgs, inputArgs.Length + 1);
                            inputArgs[5] = "";
                        }

                        return CommandType.KeySet;
                    }
                }
                    
            }
#endregion

#region LOGOUT
            // LOGOUT
            if (command == "LOGOUT")
            {
                return CommandType.Logout;
            }
#endregion
            
#region HELP
            if (string.Compare("HELP", inputArgs[0], StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                if (inputArgs.Length == 1)
                    return CommandType.Help;

                //trying get command for help
                string commandForHelp = inputArgs[1].ToUpper();
                switch (commandForHelp)
                {
                    case "INIT": return CommandType.InitHelp;
                    case "LOGIN": return CommandType.LoginHelp;
                    case "PROJECT": return CommandType.ProjectHelp;
                    case "KEY": return CommandType.KeyHelp;
                    case "LOGOUT": return CommandType.LogoutHelp;
                }

                throw new Exception("Command for help not found");
            }
#endregion

#region EXIT
            // EXIT
            if (command == "EXIT")
            {
                return CommandType.Exit;
            }
#endregion

            throw new Exception(string.Format("Command: '{0}' not found or with not correct syntax", command));
        }
    }
}
