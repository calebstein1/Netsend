using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Platform;
using Netsend.BackgroundServices;
using Netsend.Models;

namespace Netsend.UI.Common.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<FoundClientDisplay> DisplayCollection { get; }
    private const string BaseIconPath = "avares://Netsend.UI.Common/Assets/";

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
                var iconFile = c.Client.OS switch
                {
                    { } a when a.Contains("Windows") => "windows.png",
                    { } a when a.Contains("OSX") => "apple.png",
                    _ => "tencent-qq.png"
                };
                var iconPath = ImageHelper.LoadFromResource(new Uri($"{BaseIconPath}{iconFile}"));
                
                DisplayCollection.Add(new FoundClientDisplay(c.Client.Hostname, iconPath));
            }
        }

        if (e.OldItems == null) return;
        foreach (ClientInfo c in e.OldItems)
        {
            var itemToRemove = DisplayCollection.FirstOrDefault(d =>
                d.Hostname == c.Client.Hostname);
            if (itemToRemove != null)
                DisplayCollection.Remove(itemToRemove);
        }
    }
}