using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Netsend.BackgroundServices;
using Netsend.Models;

namespace Netsend.UI.Common.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<FoundClientDisplay> DisplayCollection { get; }

    public MainWindowViewModel()
    {
        DisplayCollection = new ObservableCollection<FoundClientDisplay>();

        Worker.FoundClients.CollectionChanged += ClientsUpdated;
    }

    private void ClientsUpdated(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ClientInfo c in e.NewItems)
            {
                DisplayCollection.Add(new FoundClientDisplay(c.Client.Hostname, c.Client.OS));
            }
        }

        if (e.OldItems == null) return;
        foreach (ClientInfo c in e.OldItems)
        {
            var itemToRemove = DisplayCollection.FirstOrDefault(d =>
                d.Hostname == c.Client.Hostname && d.OS == c.Client.OS);
            if (itemToRemove != null)
                DisplayCollection.Remove(itemToRemove);
        }
    }
}