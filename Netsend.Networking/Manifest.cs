using System.Runtime.InteropServices;

namespace Netsend.Networking;

public struct Manifest
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    internal string Filename;
    internal long Filesize;
}