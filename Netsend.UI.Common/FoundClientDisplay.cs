using Avalonia.Media.Imaging;
using Netsend.Models;

namespace Netsend.UI.Common;

public class FoundClientDisplay(FoundClient client, Bitmap iconPath)
{
    public FoundClient Client { get; } = client;
    public Bitmap IconPath { get; } = iconPath;
}