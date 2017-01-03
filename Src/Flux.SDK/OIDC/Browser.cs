using Flux.Logger;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SHDocVw;

namespace Flux.SDK.OIDC
{
    internal class Browser
    {
        private static readonly ILogger log = LogHelper.GetLogger("SDK|OIC|Browser");

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        private const int width = 350;
        private const int height = 525;
        private const string loginHeader = "Log In";
        private const string fluxHeader = "Flux";
        private const string authorizeHeader = "Authorize";

        private readonly BrowserType type;
        private string browserPath;

        private IntPtr NewWindowHWND = IntPtr.Zero;

        public Browser()
        {
            type = GetSystemDefaultBrowser();
        }

        internal bool NewWindowWasOpened { get; private set; }

        internal void OpenLinkInNewWindow(string url)
        {
            switch (type)
            {
                case BrowserType.IExplore:
                    {
                        NewWindowWasOpened = true;

                        var iExplorer = new SHDocVw.InternetExplorer
                        {
                            ToolBar = 0,
                            StatusBar = false,
                            MenuBar = false,
                            Width = width,
                            Height = height,
                            Visible = true
                        };

                        iExplorer.Navigate(url);
                        NewWindowHWND = new IntPtr(iExplorer.HWND);
                        BringWindowToTop(NewWindowHWND);
                    }
                    break;

                case BrowserType.Chrome:
                case BrowserType.Firefox:
                    {
                        NewWindowWasOpened = true;

                        var args = string.Format(GetNewWindowOption(), url);
                        StartBrowser(args);
                        ResizeWindow();
                    }
                    break;

                default:
                    {
                        var process = Process.Start(url);
                        if (process != null)
                            NewWindowHWND = process.MainWindowHandle;
                    }
                    break;
            }
        }

        internal void OpenLinkInNewTab(string url)
        {
            switch (type)
            {
                case BrowserType.IExplore:
                    {
                        bool found = false;
                        ShellWindows iExplorerInstances = new ShellWindows();
                        foreach (InternetExplorer iExplorer in iExplorerInstances)
                        {
                            if (iExplorer.Name == "Windows Internet Explorer")
                            {
                                iExplorer.Navigate(url, 0x800);
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            Process.Start(url);
                    }
                    break;

                case BrowserType.Chrome:
                case BrowserType.Firefox:
                    {
                        var args = string.Format(GetNewTabOption(), url);
                        StartBrowser(args);
                    }
                    break;
            }
        }

        internal void CloseLoginWindow()
        {
            if (NewWindowHWND != IntPtr.Zero)
                SendMessage(NewWindowHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void StartBrowser(string args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = browserPath,
                Arguments = args
            };

            Process.Start(startInfo);
        }

        private void ResizeWindow()
        {
            bool endProcess = false;

            // max wait is 5 sec. of process
            int maxCountCycle = 10;

            for (int i = 0; i < maxCountCycle; i++)
            {
                Process[] processes = Process.GetProcessesByName(type.ToString().ToLower());

                foreach (var process in processes.Where(p => p.MainWindowHandle != IntPtr.Zero))
                {
                    if ((process.MainWindowTitle.Contains(loginHeader) || process.MainWindowTitle.Contains(authorizeHeader))
                        && process.MainWindowTitle.Contains(fluxHeader))
                    {
                        NewWindowHWND = process.MainWindowHandle;
                        ShowWindow(process.MainWindowHandle, 1); //SW_SHOWNORMAL

                        RECT rectangle;
                        GetWindowRect(process.MainWindowHandle, out rectangle);
                        SetWindowPos(process.MainWindowHandle, 0, rectangle.Left, rectangle.Top, width, height, 0x0040); //SWP_SHOWWINDOW Displays the window.

                        endProcess = true;
                    }
                }

                if (endProcess)
                    break;

                Thread.Sleep(500);
            }
        }

        private string GetNewWindowOption()
        {
            string option = "{0}";
            switch (type)
            {
                case BrowserType.Chrome:
                    option = "--app=data:text/html,<html><body><script>window.location='{0}';</script></body></html>";
                    break;

                case BrowserType.Firefox:
                    option = "-new-window {0}";
                    break;
            }

            return option;
        }

        private string GetNewTabOption()
        {
            string option = "{0}";
            switch (type)
            {
                case BrowserType.Chrome:
                    option = "-url {0}";
                    break;

                case BrowserType.Firefox:
                    option = "-new-tab {0}";
                    break;
            }

            return option;
        }

        private BrowserType GetSystemDefaultBrowser()
        {
            browserPath = string.Empty;
            var browserType = BrowserType.Unknown;

            try
            {
                string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
                string browserPathKey = @"$BROWSER$\shell\open\command";

                browserPath = "";

                //Read default browser path from userChoiceLKey
                using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false))
                {
                    //If user choice was not found, try machine default
                    if (userChoiceKey == null)
                    {
                        //Read default browser path from Win XP registry key
                        RegistryKey browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                        //If browser path wasn¡¦t found, try Win Vista (and newer) registry key
                        if (browserKey == null)
                            browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, false);

                        if (browserKey != null)
                        {
                            var name = browserKey.GetValue(null).ToString().ToLower().Replace(((char)34).ToString(), "");
                            if (!name.EndsWith("exe"))
                                browserPath = name.Substring(0, name.LastIndexOf(".exe") + 4);

                            browserKey.Close();
                        }
                    }
                    else
                    {
                        // user defined browser choice was found
                        string progId = userChoiceKey.GetValue("ProgId").ToString();

                        // now look up the path of the executable
                        string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                        using (RegistryKey bk = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false))
                        {
                            if (bk != null)
                            {
                                var name = bk.GetValue(null).ToString().ToLower().Replace(((char)34).ToString(), "");
                                if (!name.EndsWith("exe"))
                                    browserPath = name.Substring(0, name.LastIndexOf(".exe") + 4);
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(browserPath))
                {
                    if (browserPath.ToLower().Contains(BrowserType.Chrome.ToString().ToLower()))
                        browserType = BrowserType.Chrome;
                    else if (browserPath.ToLower().Contains(BrowserType.Firefox.ToString().ToLower()))
                        browserType = BrowserType.Firefox;
                    else if ((browserPath.ToLower().Contains(BrowserType.IExplore.ToString().ToLower())))
                        browserType = BrowserType.IExplore;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return browserType;
        }
    }

    internal enum BrowserType
    {
        IExplore,
        Chrome,
        Firefox,
        Unknown
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
