using System.Net;
using System.Runtime.InteropServices;

namespace Netsend.Networking;

internal struct Identity
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    internal string Hostname;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    internal string OperatingSystem;
}
internal struct Manifest
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    internal string Filename;
    internal long Filesize;
}

internal static class StructUtils
{
    internal static byte[] GetBytesFromStruct<T>(T m) where T : notnull
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

    internal static T GetStructFromBytes<T>(byte[] a, int rSize) where T : new()
    {
        var m = new T();
        var size = Marshal.SizeOf(m);
        if (!Equals(size, rSize)) throw new InvalidOperationException();

        var p = IntPtr.Zero;
        try
        {
            p = Marshal.AllocHGlobal(size);
            Marshal.Copy(a, 0, p, size);
            m = (T)(Marshal.PtrToStructure(p, m.GetType()) ?? throw new InvalidOperationException());
        }
        finally
        {
            Marshal.FreeHGlobal(p);
        }

        return m;
    }
}