using System;
using System.Text.RegularExpressions;

namespace Flux.ConsoleApp.Helpers
{
    public static class PromptManager
    {
        public static bool ShowPrompt(string prompt)
        {
            Console.Write(prompt);
            Console.Write(@" (y/n)");

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
            } while (!Regex.IsMatch(cki.KeyChar.ToString(), "[yn]", RegexOptions.IgnoreCase));
            Console.WriteLine(cki.KeyChar);

            return Regex.IsMatch(cki.Key.ToString(), "[y]", RegexOptions.IgnoreCase);
        }
    }
}
