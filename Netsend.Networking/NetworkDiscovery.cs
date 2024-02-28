using System.Net;
using System.Net.Sockets;
using System.Text;
using Netsend.Models;

namespace Netsend.Networking;

public static class NetworkDiscovery
{
    internal static int Port = 54545;
    private static readonly UdpClient UdpClient = new();
    private static readonly UdpClient ReceivingUdpClient = new(Port);
    
    private static readonly Identity Identity = new ()
    {
        Hostname = Dns.GetHostName(),
        OperatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription
    };
    private static readonly byte[] Data = StructUtils.GetBytes(Identity);
    
    public static async Task BroadcastServiceAsync()
    {
        await UdpClient.SendAsync(Data, Data.Length, "255.255.255.255", Port);
    }

    public static async Task<FoundClient> FindServiceAsync()
    {
        var receiveBytes = await ReceivingUdpClient.ReceiveAsync();
        var returnData = StructUtils.FromBytes<Identity>(receiveBytes.Buffer, receiveBytes.Buffer.Length);

        return new FoundClient(receiveBytes.RemoteEndPoint.Address, returnData.Hostname, returnData.OperatingSystem);
    }

    public static void ShutdownService()
    {
        UdpClient.Close();
    }
}