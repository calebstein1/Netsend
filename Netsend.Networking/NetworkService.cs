using System.Net;
using System.Net.Sockets;
using System.Text;
using Netsend.Models;

namespace Netsend.Networking;

public static class NetworkService
{
    private static int _port = 54545;
    private static UdpClient _udpClient = new UdpClient();
    private static UdpClient _receivingUdpClient = new UdpClient(_port);
    private static IPEndPoint _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
    public static void BroadcastService()
    {
        var data = "I'm coming from Linux"u8.ToArray();
        _udpClient.Send(data, data.Length, "255.255.255.255", _port);
    }

    public static FoundClient FindService()
    {
        var receiveBytes = _receivingUdpClient.Receive(ref _remoteIpEndPoint);
        var returnData = Encoding.ASCII.GetString(receiveBytes);

        return new FoundClient(returnData, _remoteIpEndPoint.Address);
    }
}