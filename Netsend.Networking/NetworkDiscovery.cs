using System.Net;
using System.Net.Sockets;
using System.Text;
using Netsend.Models;

namespace Netsend.Networking;

public static class NetworkDiscovery
{
    private static int _port = 54545;
    private static UdpClient _udpClient = new UdpClient();
    private static UdpClient _receivingUdpClient = new UdpClient(_port);
    private static IPEndPoint _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    
    public static void BroadcastService()
    {
        var hostname= Dns.GetHostName();
        var os = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        var data = $"{hostname},{os}".Select(c => (byte)c).ToArray();
        _udpClient.Send(data, data.Length, "255.255.255.255", _port);
    }

    public static FoundClient FindService()
    {
        var receiveBytes = _receivingUdpClient.Receive(ref _remoteIpEndPoint);
        var returnData = Encoding.ASCII.GetString(receiveBytes).Split(',');

        return new FoundClient(_remoteIpEndPoint.Address, returnData[0], returnData[1]);
    }
}