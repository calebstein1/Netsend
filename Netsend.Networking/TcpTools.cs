using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Netsend.Networking;

public class TcpTools
{
    public readonly ObservableString TcpStatus = new();
    private int _port = NetworkDiscovery.Port;
    
    public async Task SendRequestAsync(IPAddress ipAddress, Uri filePath)
    {
        var ipEndPoint = new IPEndPoint(ipAddress, _port);
        using TcpClient client = new();

        TcpStatus.Value = $"Sending {filePath.ToString()} to system at {ipAddress.ToString()}...";
        await client.ConnectAsync(ipEndPoint);
        await using var stream = client.GetStream();

        var conSuccessBuffer = new byte[1];
        var bytesRead = await stream.ReadAsync(conSuccessBuffer);

        if (Equals(bytesRead, 1))
        {
            var fileName = filePath.ToString();
            var fileNameByes = Encoding.UTF8.GetBytes(fileName);
            await stream.WriteAsync(fileNameByes);
        }

        TcpStatus.Value = $"Sent {filePath.ToString()} to system at {ipAddress.ToString()}";
    }

    public async Task ListenForRequestsAsync()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Any, _port);
        TcpListener listener = new(ipEndPoint);

        while (true)
        {
            try
            {
                listener.Start();

                using var handler = await listener.AcceptTcpClientAsync();
                await using var stream = handler.GetStream();

                var isConnected = new byte[] { 1 };
                await stream.WriteAsync(isConnected);
                
                var checksumBuffer = new byte[1_024];
                var checksumLen = await stream.ReadAsync(checksumBuffer);
                
                var checksum = Encoding.UTF8.GetString(checksumBuffer, 0, checksumLen);
                TcpStatus.Value = $"Got {checksum} from remote system";
                await Task.Delay(5000);
                TcpStatus.Value = "Netsend ready";
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}