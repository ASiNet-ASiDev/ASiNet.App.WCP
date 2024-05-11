using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows;
using IWshRuntimeLibrary;

namespace ASiNet.App.WCP.Desktop;
public static class ServiceContext
{
    static ServiceContext()
    {
        if (!CheckActualService())
            InstallOrReinstallService();
    }

    private const string WCP_SERVICE_NAME = "WCPService.exe";
    private const string WCP_SERVICE_LNK = "WCPService.lnk";

    private static string _serviceDirectory = Path.Join(Environment.CurrentDirectory, "service");
    private static string _cnfPath = Path.Join(Environment.CurrentDirectory, "configs", "config.cnf");
    private static string _servicePath = Path.Join(_serviceDirectory, WCP_SERVICE_NAME);

    public static IEnumerable<string> IpAddresses => GetLocalIPv4(NetworkInterfaceType.Ethernet).Concat(GetLocalIPv4(NetworkInterfaceType.Wireless80211));

    public static bool Autorun
    {
        get
        {
            // TODO add to autorun
            return ExistAutorun();
        }
        set
        {
            if (value)
                CreateAutorun();
            else
                RemoveAutorun();
        }
    }

    public static bool IsRun => CheckRun();

    public static async Task<bool> RunService()
    {
        return await Task.Run(() =>
        {
            try
            {
                if (IsRun)
                    return true;
                var prc = Process.Start(_servicePath);
                prc.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        });
    }

    public static async Task<bool> StopService()
    {
        return await Task.Run(() =>
        {
            try
            {
                using var process = GetServiceProccess();
                if (process is null)
                    return true;
                process.Kill();
                process.WaitForExit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        });
    }

    private static bool CheckRun()
    {
        using var process = GetServiceProccess();
        return process is not null;
    }

    private static Process? GetServiceProccess()
    {
        var sp = _servicePath.Replace('\\', '/');
        foreach (var proccess in Process.GetProcessesByName("WCPService"))
        {
            var fileName = proccess.MainModule?.FileName;
            if (fileName is null)
            {
                proccess.Dispose();
                continue;
            }
            if (fileName.Replace('\\', '/') == sp)
                return proccess;
            proccess.Dispose();
        }
        return null;
    }

    public static IEnumerable<string> GetLocalIPv4(NetworkInterfaceType _type)
    {

        var ni = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        yield return ip.Address.ToString();
                    }
                }
            }
    }


    public static void CreateAutorun()
    {
        try
        {
            var autorunPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Startup), WCP_SERVICE_LNK);
            WshShell wshShell = new WshShell();

            IWshShortcut Shortcut = (IWshShortcut)wshShell.
                CreateShortcut(autorunPath);

            Shortcut.TargetPath = _servicePath;

            Shortcut.Save();
        }
        catch { }
    }

    public static bool ExistAutorun()
    {
        return System.IO.File.Exists(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Startup), WCP_SERVICE_LNK));
    }

    public static void RemoveAutorun()
    {
        try
        {
            var autorunPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Startup), WCP_SERVICE_LNK);
            System.IO.File.Delete(autorunPath);
        }
        catch { }
    }


    public static bool InstallOrReinstallService()
    {
        try
        {
            if(IsRun)
                return true;
            if (!Directory.Exists(_serviceDirectory))
                Directory.CreateDirectory(_serviceDirectory);
            if(System.IO.File.Exists(_cnfPath))
                System.IO.File.Delete(_cnfPath);
            RemoveAutorun();
            var uri = new Uri($"Service\\service.zip", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            using var stream = streamInfo.Stream;
            using var zip = new ZipArchive(stream);
            zip.ExtractToDirectory(_serviceDirectory);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool CheckActualService()
    {
        try
        {
            if(System.IO.File.Exists(_servicePath))
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
