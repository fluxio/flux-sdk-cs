using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Flux.SDK;
using Flux.SDK.DataTableAPI;
using Flux.SDK.DataTableAPI.DatatableTypes;
using Flux.SDK.Types;
using Flux.Serialization;
using Flux.ConsoleApp.Helpers;
using Flux.ConsoleApp.Properties;

namespace Flux.ConsoleApp.Commands
{
    public static partial class Command
    {
        private static readonly Dictionary<string, string> GeometricPrimitives = new Dictionary<string, string>();
        static Command()
        {
            GeometricPrimitives.Add("ARC", PrimitivesResource.arc);
            GeometricPrimitives.Add("BLOCK", PrimitivesResource.block);
            GeometricPrimitives.Add("CIRCLE", PrimitivesResource.circle);
            GeometricPrimitives.Add("CURVE", PrimitivesResource.curve);
            GeometricPrimitives.Add("ELLIPSE", PrimitivesResource.ellipse);
            GeometricPrimitives.Add("LINE", PrimitivesResource.line);
            GeometricPrimitives.Add("MESH", PrimitivesResource.mesh);
            GeometricPrimitives.Add("POINT", PrimitivesResource.point);
            GeometricPrimitives.Add("POLYCURVE", PrimitivesResource.polycurve);
            GeometricPrimitives.Add("POLYLINE", PrimitivesResource.polyline);
            GeometricPrimitives.Add("RECTANGLE", PrimitivesResource.rectangle);
            GeometricPrimitives.Add("SPHERE", PrimitivesResource.sphere);
        }

        public static void GetKeyList(string projectId)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            List<CellInfo> keys = project.DataTable.Cells;

            if (keys.Any())
            {
                for (int i = 0; i < keys.Count; i++)
                    Console.WriteLine("Key #{0}: id={1}, label='{2}', description='{3}'", i, keys[i].CellId, keys[i].ClientMetadata.Label, keys[i].ClientMetadata.Description);
            }
            else
            {
                Console.WriteLine("No keys in this project yet.");
                Console.WriteLine("Please use 'PROJECT {id Project} KEY CREATE {label} [description] {value}' command to create the key.");
            }
        }

        public static void CreateNewKey(string projectId, string label, string description, string value)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            var clientMetadata = new ClientMetadata
            {
                Label = label,
                Description = description,
                Locked = false
            };

            CellInfo createdKey = project.DataTable.CreateCell(value, clientMetadata);

            Console.WriteLine("New key '{0}' was created.", createdKey.ClientMetadata.Label);
        }

        public static void DeleteKey(string projectId, string keyId)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            CellInfo key = project.DataTable.Cells.FirstOrDefault(c => c.CellId == keyId);
            if (key == null)
                throw new Exception(string.Format("Key by id: '{0}' not found.", keyId));

            string prompt =
                string.Format(
                    "You actually want delete key by label: '{0}'{1}(you won't be able recovery infomation after this procedure)"
                    , key.ClientMetadata.Label
                    , Environment.NewLine);

            if (PromptManager.ShowPrompt(prompt))
            {
                key = project.DataTable.DeleteCell(keyId);
                Console.WriteLine("Key '{0}' by label: '{1}' has been deleted", key.CellId, key.ClientMetadata.Label);
            }
        }

        public static void GetKey(string projectId, string keyId, string fileName)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            CellInfo key = project.DataTable.Cells.FirstOrDefault(c => c.CellId == keyId);
            if (key == null)
                throw new Exception(string.Format("Key by id: '{0}' not found.", keyId));

            //should be GetCell
            Cell existingKey = project.DataTable.GetCell(keyId, true, true);
            string jsonStr = StreamUtils.GetStringFromStream(existingKey.Value.Stream, existingKey.Value.StreamLength);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (FileHelper.SaveToFile(ref fileName, jsonStr))
                    Console.WriteLine("Key by label: '{0}' was saved in file: '{1}'", existingKey.Info.ClientMetadata.Label, fileName);
                else
                    Console.WriteLine("File '{0}' was not saved", fileName);
            }
            else
            {
                Console.WriteLine("Key by label: '{0}' has value:", existingKey.Info.ClientMetadata.Label);
                Console.WriteLine(jsonStr);
            }
        }

        public static void SetKey(string projectId, string keyId, string fileNameOrValueOrGeometry)
        {
            CheckForInitAndLoginSDK();

            Project project = GetProjectById(projectId);

            CellInfo key = project.DataTable.Cells.FirstOrDefault(c => c.CellId == keyId);
            if (key == null)
                throw new Exception(string.Format("Key by id: '{0}' not found.", keyId));

            string valueForSet;
            SourceOfData sourceOfData = SourceOfData.Primitive;
            string name = string.Empty;

            //checking parameter as filename 
            string fileName = string.Empty;
            if (fileNameOrValueOrGeometry.StartsWith("<") && fileNameOrValueOrGeometry.Length > 1)
            {
                //delete first char
                fileName = fileNameOrValueOrGeometry.Substring(1);
                if (!FileHelper.IsFileExists(ref fileName))
                    Console.WriteLine("File name '{0}' is not exists", fileName);

                valueForSet = File.ReadAllText(fileName);
                sourceOfData = SourceOfData.File;
            }
            else
            {
                name = fileNameOrValueOrGeometry.ToUpper();

                //check parameter, if name is inside Geometric Primitives
                if (!GeometricPrimitives.TryGetValue(name, out valueForSet))
                {
                    //check parameter, if number is inside Geometric Primitives 
                    int numberPrimitive;
                    if (int.TryParse(name, out numberPrimitive) && numberPrimitive <= GeometricPrimitives.Count)
                    {
                        valueForSet = GeometricPrimitives.Values.ElementAt(numberPrimitive - 1);
                        name = GeometricPrimitives.Keys.ElementAt(numberPrimitive - 1);
                    }
                    else
                    {
                        valueForSet = fileNameOrValueOrGeometry;
                        sourceOfData = SourceOfData.Value;
                    }
                }
            }

            var serializedValueStr = DataSerializer.Serialize(valueForSet);
            Stream stream = StreamUtils.GenerateStreamFromString(serializedValueStr);
            CellInfo updatingKey = project.DataTable.SetCell(key.CellId, stream, key.ClientMetadata);

            switch (sourceOfData)
            {
                case SourceOfData.File:
                    Console.WriteLine("Key '{0}' was updated from file: '{1}'", updatingKey.ClientMetadata.Label, fileName);
                    break;

                case SourceOfData.Primitive:
                    Console.WriteLine("Key '{0}' was updated by predefined geometric primitive '{1}': ", updatingKey.ClientMetadata.Label, name);
                    Console.WriteLine(@"Please vivsit https://flux.io, project - '{0}', key - '{1}' to review this changes.", project.Name, key.ClientMetadata.Label);
                    break;

                case SourceOfData.Value:
                    Console.WriteLine("Key '{0}' was updated with value: '{1}'", updatingKey.ClientMetadata.Label, valueForSet);
                    break;
            }
        }

        public static void KeyHelp()
        {
            //make list Geometric Primitives for help
            var helpGeometricPrimitives = new StringBuilder();
            for (int i = 0; i < GeometricPrimitives.Keys.Count; i++)
            {
                helpGeometricPrimitives.AppendFormat("\t{0} {1}{2}", i+1, GeometricPrimitives.Keys.ElementAt(i).ToLower(), Environment.NewLine);
            }

            string help =
@"Helping work with keys (list, create, delete, get, set) are available for the currently logged in user.

PROJECT {id Project} KEY LIST
PROJECT {id Project} KEY CREATE {label} [description] {value}
PROJECT {id Project} KEY DELETE {id key}
PROJECT {id Project} KEY {id key} GET [{file name}]
PROJECT {id Project} KEY {id key} SET {value}|<{file name}|{predefined primitive names}

You have options to set up a value:
    - load value from file
    - pass directly
    - use one of the predefined primitive names or this number:
" + helpGeometricPrimitives +
@"
For example: 
    project 'projectId' KEY LIST
    project 'projectId' key create ""My fist key"" ""test value here""
    project 'projectId' key 'keyId' delete
    project 'projectId' key 'keyId' get

Set up value from file
    project 'projectId' key 'keyId' set <""File.txt""
Set up pass directly
    project 'projectId' key 'keyId' set ""value for save""
Set up from predefined primitive names by name
    project 'projectId' key 'keyId' set arc 
Set up from predefined primitive names by number
    project 'projectId' key 'keyId' set 1
";

            Console.WriteLine(help);
        }

        private enum SourceOfData
        {
            Value,
            File,
            Primitive
        }
    }
}
