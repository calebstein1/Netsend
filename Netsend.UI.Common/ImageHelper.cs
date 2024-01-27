using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Netsend.UI.Common;

internal static class ImageHelper
{
    internal static Bitmap LoadFromResource(Uri resourceUri)
    {
        return new Bitmap(AssetLoader.Open(resourceUri));
    }
}