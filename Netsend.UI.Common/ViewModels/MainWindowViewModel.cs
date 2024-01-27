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
    private const string BaseIconPath = "avares://Netsend.UI.Common/Assets";

    public MainWindowViewModel()
    {
        DisplayCollection = new ObservableCollection<FoundClientDisplay>();

        Worker.FoundClients.CollectionChanged += ClientsUpdated;
    }

    private void ClientsUpdated(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // The idea here is to track our own list to display, rather than pull the list from BackgroundServices.Worker directly.
        // This is good because we don't need things like the pingCounter in the UI, and the backend logic doesn't care about
        // UI-specific things like the OS icon file. It also smooths over threading concerns between the Worker and UI threads.
        if (e.NewItems != null)
        {
            foreach (IFoundClientExt c in e.NewItems)
            {
                // I find switch expressions to be more clear than big if/else if/else blocks; this is just grabbing the proper
                // icon file based in the information in the OS string.
                var iconFile = c.Client.OS switch
                {
                    { } a when a.Contains("Windows") => "windows.png",
                    { } a when a.Contains("Mac") => "apple.png",
                    _ => "tencent-qq.png"
                };
                var iconPath = ImageHelper.LoadFromResource(new Uri($"{BaseIconPath}/{iconFile}"));
                
                DisplayCollection.Add(new FoundClientDisplay(c.Client, iconPath));
            }
        }

        // We're using the interface IFoundClientExt here for clarity, since technically c and d are of different types
        // (ClientInfo and FoundClientDisplay, respectively), but they both implement the Client property as returning
        // a FoundClient object (hence IFoundClientExt -- FoundClient with extensions). It's not strictly-speaking necessary,
        // but I find the cognitive load lower than trying to figure out why we're comparing ClientInfo with FoundClientDisplay.
        if (e.OldItems == null) return;
        foreach (IFoundClientExt c in e.OldItems)
        {
            var itemToRemove = DisplayCollection.FirstOrDefault(d =>
                Equals(d.Client.Hostname, c.Client.Hostname) && Equals(d.Client.Address, c.Client.Address));
            if (itemToRemove != null)
                DisplayCollection.Remove(itemToRemove);
        }
    }
}