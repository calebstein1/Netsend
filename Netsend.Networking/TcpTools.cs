using System.Net;
using System.Net.Sockets;

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
        if (await stream.ReadAsync(conSuccessBuffer) != 1) throw new InvalidOperationException();

        if (Equals(conSuccessBuffer[0], (byte)1))
        {
            TcpStatus.Value = "Generating transfer manifest...";
            var manifest = new Manifest
            {
                Filename = Path.GetFileName(filePath.LocalPath),
                Filesize = new FileInfo(filePath.LocalPath).Length
            };
            await stream.WriteAsync(StructUtils.GetBytesFromStruct(manifest));
        }

        var remotePreparedBuffer = new byte[1];
        if (await stream.ReadAsync(remotePreparedBuffer) != 1) throw new InvalidOperationException();

        if (Equals(remotePreparedBuffer[0], (byte)1))
        {
            TcpStatus.Value = $"Ok... sending {filePath.LocalPath}";
        }
        else
        {
            TcpStatus.Value = "Remote client rejected transmission";
        }
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

                var successValue = new byte[] { 1 };
                await stream.WriteAsync(successValue);
                
                var manifestBuffer = new byte[1_024];
                var manifestSize = await stream.ReadAsync(manifestBuffer);

                var manifest = StructUtils.GetStructFromBytes<Manifest>(manifestBuffer, manifestSize);
                var fileName = manifest.Filename;
                var documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
                var prefix = 0;

                while (Directory.GetFileSystemEntries(documentsDir, "*", SearchOption.TopDirectoryOnly)
                       .Select(Path.GetFileName)
                       .Contains(fileName))
                {
                    prefix++;
                    fileName = $"{prefix}_{manifest.Filename}";
                }
                
                TcpStatus.Value = $"Got {fileName} from remote system, size: {manifest.Filesize.ToString()} bytes";

                var confirmOkToSend = new byte[] { 1 };
                await stream.WriteAsync(confirmOkToSend);
                
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