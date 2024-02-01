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

        var buffer = new byte[1_024];
        var isAcceptedLen = await stream.ReadAsync(buffer);

        var isAcceptedStr = Encoding.UTF8.GetString(buffer, 0, isAcceptedLen);
        if (Equals(isAcceptedStr, "accepted"))
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

                var isAccepted = "accepted";
                var isAcceptedBytes = Encoding.UTF8.GetBytes(isAccepted);
                await stream.WriteAsync(isAcceptedBytes);
                
                var buffer = new byte[1_024];
                var fileNameLen = await stream.ReadAsync(buffer);
                
                var fileNameStr = Encoding.UTF8.GetString(buffer, 0, fileNameLen);
                TcpStatus.Value = $"Got {fileNameStr} from remote system";
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