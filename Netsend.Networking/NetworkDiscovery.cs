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
    private static IPEndPoint _remoteIpEndPoint = new(IPAddress.Any, 0);
    
    private static readonly string Hostname= Dns.GetHostName();
    private static readonly string OperatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
    private static readonly byte[] Data = $"{Hostname},{OperatingSystem}".Select(c => (byte)c).ToArray();
    
    public static void BroadcastService()
    {
        UdpClient.Send(Data, Data.Length, "255.255.255.255", _port);
    }

    public static FoundClient FindService()
    {
        var receiveBytes = ReceivingUdpClient.Receive(ref _remoteIpEndPoint);
        var returnData = Encoding.ASCII.GetString(receiveBytes).Split(',');

        return new FoundClient(_remoteIpEndPoint.Address, returnData[0], returnData[1]);
    }
}