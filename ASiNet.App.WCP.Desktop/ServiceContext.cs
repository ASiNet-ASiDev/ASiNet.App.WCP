using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ASiNet.App.WCP.Desktop;
public static class ServiceContext
{

    private static string _servicePath = Path.Join(Environment.CurrentDirectory, "service", "WCPService.exe");

    public static IEnumerable<string> IpAddresses => GetLocalIPv4(NetworkInterfaceType.Ethernet).Concat(GetLocalIPv4(NetworkInterfaceType.Wireless80211));

    public static bool Autorun
    {
        get
        {
            // TODO add to autorun
            return false;
        }
        set
        {
            // TODO add to autorun
        }
    }

    public static bool IsRun => CheckRun();

    public static async Task<bool> RunService()
    {
        return await Task.Run(() =>
        {
            try
            {
                if(IsRun)
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
}
