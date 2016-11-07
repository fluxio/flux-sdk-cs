using System;
using System.IO;

namespace Flux.ConsoleApp.Helpers
{
    public static class FileHelper
    {
        private static string BaseDirectory { get; set; }

        static FileHelper()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public static bool SaveToFile(ref string fileName, string content)
        {
            if (IsPathValid(ref fileName))
            {
                File.WriteAllText(fileName, content);
                return true;
            }
            return false;
        }

        public static bool IsPathValid(ref string fileName)
        {
            string directoryName;

            try
            {
                directoryName = Path.GetDirectoryName(fileName);
                if (string.IsNullOrEmpty(Path.GetPathRoot(directoryName)))
                    directoryName = string.Empty;
            }
            catch
            {
                directoryName = string.Empty;
            }

            if (string.IsNullOrEmpty(directoryName))
                fileName = string.Format("{0}{1}", BaseDirectory, fileName);
            
            Uri pathUri;
            Boolean isValidUri = Uri.TryCreate(fileName, UriKind.Absolute, out pathUri);

            return isValidUri && pathUri != null && pathUri.IsLoopback;
        }

        public static bool IsFileExists(ref string fileName)
        {
            if (IsPathValid(ref fileName))
            {
                if (!File.Exists(fileName))
                {
                    string fileNameJson = string.Format("{0}.json", fileName);
                    if (File.Exists(fileNameJson))
                        fileName = fileNameJson;
                }

                return File.Exists(fileName);
            }
            return  false;
        }
    }
}
