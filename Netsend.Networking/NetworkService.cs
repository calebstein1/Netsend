using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Netsend.Networking;

public static class NetworkService
{
    public static string Message = "Nothing yet";
    public static string BroadcastService()
    {
        var port = 54545;
        var udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        var data = "ABCD"u8.ToArray();
        udpClient.Send(data, data.Length, "255.255.255.255", port);
        udpClient.Close();

        return "message sent";
    }
}