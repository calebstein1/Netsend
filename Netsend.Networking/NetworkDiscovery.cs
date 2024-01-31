using System.Net;
using System.Net.Sockets;
using System.Text;
using Netsend.Models;

namespace Netsend.Networking;

public static class NetworkDiscovery
{
    private static int _port = 54545;
    private static readonly UdpClient UdpClient = new();
    private static readonly UdpClient ReceivingUdpClient = new(_port);
    
    private static readonly string Hostname = Dns.GetHostName();
    private static readonly string OperatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
    private static readonly byte[] Data = $"{Hostname},{OperatingSystem}".Select(c => (byte)c).ToArray();
    
    public static async Task BroadcastServiceAsync()
    {
        await UdpClient.SendAsync(Data, Data.Length, "255.255.255.255", _port);
    }

    public static async Task<FoundClient> FindServiceAsync()
    {
        var receiveBytes = await ReceivingUdpClient.ReceiveAsync();
        var returnData = Encoding.ASCII.GetString(receiveBytes.Buffer).Split(',');

        return new FoundClient(receiveBytes.RemoteEndPoint.Address, returnData[0], returnData[1]);
    }

    public static void ShutdownService()
    {
        UdpClient.Close();
    }
}