using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Flux.ConsoleApp.Commands;
using Flux.ConsoleApp.Enums;

namespace Flux.ConsoleApp
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("FluxSDK Console Application, version 1.0.0.0");
            Console.WriteLine("Copyright © Flux Factory, Inc. 2016");
            Console.WriteLine("Type 'help' to see help.");
            Console.WriteLine("Type 'help name' to find out more about the function 'name'.");

            Console.Title = "FluxSDK ConsoleApp";

            bool exit = false;
            while (!exit)
            {
                string[] inputArgs = ReadFromConsole("> ");

                try
                {
                    CommandType commandType = CommandParser.ParseArguments(ref inputArgs);

                    switch (commandType)
                    {
                        case CommandType.Init:
                            Command.Init(clientId: inputArgs[1], clientVersion: inputArgs[2]);
                            break;
                        case CommandType.InitHelp:
                            Command.InitHelp();
                            break;

                        case CommandType.Login:
                            Command.Login(clientSecret: inputArgs[1], serverUrl: inputArgs[2]);
                            break;
                        case CommandType.LoginHelp:
                            Command.LoginHelp();
                            break;

                        case CommandType.ProjectList:
                            Command.ProjectList();
                            break;
                        case CommandType.ProjectCreate:
                            Command.CreateProject(projectName: inputArgs[2]);
                            break;
                        case CommandType.ProjectDelete:
                            Command.DeleteProject(projectId: inputArgs[2]);
                            break;
                        case CommandType.ProjectHelp:
                            Command.ProjectHelp();
                            break;

                        case CommandType.KeyList:
                            Command.GetKeyList(projectId: inputArgs[1]);
                            break;
                        case CommandType.KeyCreate:
                            Command.CreateNewKey(projectId: inputArgs[1], label: inputArgs[4], description: inputArgs[5], value: inputArgs[6]);
                            break;
                        case CommandType.KeyDelete:
                            Command.DeleteKey(projectId: inputArgs[1], keyId: inputArgs[4]);
                            break;
                        case CommandType.KeyGet:
                            Command.GetKey(projectId: inputArgs[1], keyId: inputArgs[3], fileName: inputArgs[5]);
                            break;
                        case CommandType.KeySet:
                            Command.SetKey(projectId: inputArgs[1], keyId: inputArgs[3], fileNameOrValueOrGeometry: inputArgs[5]);
                            break;
                        case CommandType.KeyHelp:
                            Command.KeyHelp();
                            break;

                        case CommandType.Logout:
                            Command.Logout();
                            break;
                        case CommandType.LogoutHelp:
                            Command.LogoutHelp();
                            break;

                        case CommandType.Help:
                            Command.Help();
                            break;

                        case CommandType.Exit:
                            exit = true;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR(S): {0}", e.Message);
                    Console.WriteLine(Environment.NewLine + "For more information on a specific command, type HELP command-name");
                }
            }
        }

        private static string[] ReadFromConsole(string promptMessage = "")
        {
            string inputStr;

            do
            {
                Console.Write(Environment.NewLine + "fluxConsole" + promptMessage);
                inputStr = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(inputStr));

            //prepare input string for parsing parameters with spaces 
            var regex = new Regex(@"(.*?)""([^""]*)""(.*)");
            Match match = regex.Match(inputStr);
            while (match.Success)
            {
                //if have empty parameter, for example: ""
                if (string.IsNullOrEmpty(match.Groups[2].Value))
                    inputStr = string.Format("{0}0{1}{2}", match.Groups[1].Value, '+', match.Groups[3].Value);
                else
                    inputStr = string.Format("{0}{1}{2}", match.Groups[1].Value, match.Groups[2].Value.Replace(" ", "+"), match.Groups[3].Value);

                match = regex.Match(inputStr);
            }

            //cleaning parameters and parse parameters with spaces 
            List<string> resultList = new List<string>();

            string[] strings = inputStr.Split(' ');
            foreach (string value in strings)
            {
                if(string.IsNullOrWhiteSpace(value))
                    continue;

                string cleanValue = value.Trim().Replace("0+", string.Empty).Replace("+", " ");

                resultList.Add(cleanValue);
            }

            return resultList.ToArray();
        }
    }
}
