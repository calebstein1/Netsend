using Netsend.Networking;

namespace Netsend.UI.Common.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
    public string Greeting => NetworkService.BroadcastService();
#pragma warning restore CA1822 // Mark members as static
}