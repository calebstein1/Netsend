using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;

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
            TcpStatus.Value = "Generating transfer manifest...";
            var manifest = new Manifest
            {
                Filename = Path.GetFileName(filePath.LocalPath),
                Filesize = new FileInfo(filePath.LocalPath).Length
            };
            await stream.WriteAsync(GetBytes(manifest));
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
                
                var manifestBuffer = new byte[1_024];
                var manifestSize = await stream.ReadAsync(manifestBuffer);

                var manifest = FromBytes(manifestBuffer, manifestSize);
                TcpStatus.Value = $"Got {manifest.Filename} from remote system, size: {manifest.Filesize.ToString()} bytes";
                await Task.Delay(5000);
                TcpStatus.Value = "Netsend ready";
            }
            finally
            {
                listener.Stop();
            }
        }
    }

    private static byte[] GetBytes(Manifest m)
    {
        var size = Marshal.SizeOf(m);
        var a = new byte[size];

        var p = IntPtr.Zero;
        try
        {
            p = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(m, p, true);
            Marshal.Copy(p, a, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(p);
        }

        return a;
    }

    private static Manifest FromBytes(byte[] a, int rSize)
    {
        var m = new Manifest();
        var size = Marshal.SizeOf(m);
        if (!Equals(size, rSize)) throw new InvalidOperationException();

        var p = IntPtr.Zero;
        try
        {
            p = Marshal.AllocHGlobal(size);
            Marshal.Copy(a, 0, p, size);
            m = (Manifest)(Marshal.PtrToStructure(p, m.GetType()) ?? throw new InvalidOperationException());
        }
        finally
        {
            Marshal.FreeHGlobal(p);
        }

        return m;
    }
}