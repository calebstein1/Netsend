using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Netsend.BackgroundServices;
using Netsend.Networking;
using Netsend.UI.Common.DataModel;
using Netsend.UI.Common.Services;
using ReactiveUI;

namespace Netsend.UI.Common.ViewModels;

public class FoundClientsListViewModel : ViewModelBase
{
    public readonly FoundClientsDisplayFactory Factory;
    public readonly TcpTools Tcp = new();
    
    public FoundClientsListViewModel()
    {
        Worker.FoundClients.CollectionChanged += (sender, e) =>
            FoundClientsService.ClientsUpdated(sender, e, this);
        Task.Run(() => Tcp.ListenForRequestsAsync());
        Tcp.TcpStatus.PropertyChanged += (sender, e) =>
            Status = Tcp.TcpStatus.Value ?? string.Empty;
        Factory = new FoundClientsDisplayFactory(this);
    }
    
    private ObservableCollection<FoundClientDisplay> _displayCollection = [];
    public ObservableCollection<FoundClientDisplay> DisplayCollection
    {
        get => _displayCollection;
        set => this.RaiseAndSetIfChanged(ref _displayCollection, value);
    }

    private string _status = "Netsend ready";
    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    internal void ResetStatus()
    {
        Status = "Netsend ready";
    }
}