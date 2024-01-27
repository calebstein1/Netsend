using Avalonia.Media.Imaging;

namespace Netsend.UI.Common;

public class FoundClientDisplay(string hostname, Bitmap iconPath)
{
    public string Hostname { get; } = hostname;
    public Bitmap IconPath { get; } = iconPath;
}