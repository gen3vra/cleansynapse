using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows.Forms;
using Razer.Emily.Common;
using Razer.Emily.UI;
//using Razer.Emily.UISDK;
using Razer.Storage;

class Program
{
    #region Hiding from taskbar
    [DllImport("user32.dll")]
    static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    [DllImport("user32.dll")]
    static extern IntPtr GetShellWindow();
    internal const UInt32 SC_CLOSE = 0xF060;
    internal const UInt32 MF_GRAYED = 0x00000001;
    #endregion
    static string defaultDirectory = @"C:\Program Files (x86)\Razer\Synapse";
    static void Main(string[] args)
    {
        #region Hide program from taskbar
        IntPtr current = Process.GetCurrentProcess().MainWindowHandle;
        EnableMenuItem(GetSystemMenu(current, false), SC_CLOSE, MF_GRAYED);
        IntPtr shellWin = GetShellWindow();
        SetParent(current, shellWin);
        #endregion
        if (File.Exists(AssemblyLocation(true) + "\\RzSynapse.exe"))
        {
            MainStorage mainApp = Singleton<MainStorage>.Instance;
            mainApp.Initialize(Storage_Project.Emily);
            if (!mainApp.TryLastLogin())
                MessageBox.Show("Please login on Razer Synapse first.");
            else
            {
                RzDeviceManager dManager = new RzDeviceManager();
                dManager.Enumerate();
                CommonConfigLoader configLoader = Singleton<CommonConfigLoader>.Instance;
                configLoader.StartConfig();
                RzDevice[] allDevices = dManager.ActiveDevices.ToArray();
                foreach (RzDevice rzDevice in allDevices)
                {
                    rzDevice.RefreshData();
                    configLoader.DeviceAdded(rzDevice.VID, rzDevice.PID);
                    //PluginDevice pluginDevice = configLoader.FindDevice(rzDevice.PID);
                    //Console.WriteLine("Loaded: " + pluginDevice.Name);
                }
            }
        }
        else
        {
            DialogResult result = DialogResult.None;
            if (args.Length < 1)
            {
                if (Directory.Exists(defaultDirectory))
                    result = MessageBox.Show("The program is not in the Synapse folder. Automatically move it?", "Synapse Was Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                else
                {
                    MessageBox.Show("The program is not in the Synapse folder, and we can't find the directory. You probably installed it somewhere else. Please manually move the program to the root of the Razer Synapse directory.");
                    return;
                }
            }
            if (result == DialogResult.Yes || args.Length > 0)
            {
                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
                if (!hasAdministrativeRight)
                {
                    string fileName = Assembly.GetExecutingAssembly().Location;
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        Verb = "runas",
                        Arguments = "-true",
                        FileName = fileName
                    };
                    try
                    {
                        Process.Start(processInfo);
                    }
                    catch
                    {
                        //Thrown if cancelled.
                        MessageBox.Show("The program needs admin to move the file to your program files.");
                        MessageBox.Show("The program will now exit.");
                        return;
                    }
                    return;
                }
                else
                {
                    string path = defaultDirectory + "\\" + Path.GetFileName(AssemblyLocation(false));
                    File.Copy(AssemblyLocation(false), path, true);
                    Process.Start(path);
                    result = MessageBox.Show("Run program at computer startup? Please disable Synapse from running at startup if yes.", "Add Startup Shortcut", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        AddShortcut("CleanSynapse", @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup");
                    }
                    MessageBox.Show("Completed installation.");
                    return;
                }
            }
            if (result == DialogResult.No)
            {
                MessageBox.Show("The program will now exit.");
                return;
            }
        }
    }

    public static string AssemblyLocation(bool directory)
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return directory ? Path.GetDirectoryName(path) : path;
    }

    private static void AddShortcut(string linkName, string dir)
    {
        string deskDir = dir;
        try
        {
            File.Delete(dir);
        }
        catch { }
        using (StreamWriter writer = new StreamWriter(deskDir + "\\" + linkName + ".url"))
        {
            string app = Assembly.GetExecutingAssembly().Location;
            writer.WriteLine("[InternetShortcut]");
            writer.WriteLine("URL=file:///" + app);
            writer.WriteLine("IconIndex=0");
            string icon = app.Replace('\\', '/');
            writer.WriteLine("IconFile=" + icon);
            writer.Flush();
        }
    }
}