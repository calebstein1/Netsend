using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Netsend.Models;
using Netsend.UI.Common.ViewModels;
using Netsend.Networking;
using ReactiveUI;

namespace Netsend.UI.Common.DataModel;

public class FoundClientDisplay(FoundClient client, Bitmap iconPath, FoundClientsListViewModel viewModel) : IFoundClientExt
{
    public FoundClient Client { get; } = client;
    public Bitmap IconPath { get; } = iconPath;
    private FoundClientsListViewModel ViewModel { get; } = viewModel;

    // I really don't like having all this logic in the DataModel, but try as I might, I haven't been able to make anything
    // work as cleanly as this. It's unfortunate, but for now this is just how it has to be.
    public ReactiveCommand<FoundClientDisplay, Task> SendFileCommand { get; } =
        ReactiveCommand.Create<FoundClientDisplay, Task>(SendFileAsync);
    /*public ReactiveCommand<FoundClientDisplay, Task> SendDirectoryCommand { get; } =
        ReactiveCommand.Create<FoundClientDisplay, Task>(SendDirectoryAsync);*/
    
    private static async Task SendFileAsync(FoundClientDisplay client)
    {
        client.ViewModel.Status = $"Sending file to {client.Client.Hostname}...";
        var storageProvider = new Window().StorageProvider;
        var file = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Netsend"
        });
        client.ViewModel.Status = $"You selected {file[0].Path}";
        await client.ViewModel.Tcp.SendRequestAsync(client.Client.Address, file[0].Path);
        await Task.Delay(5000); // This is to simulate the eventual file-sending logic
        client.ViewModel.ResetStatus();
    }
    
    private static async Task SendDirectoryAsync(FoundClientDisplay client)
    {
        client.ViewModel.Status = $"Sending directory to {client.Client.Hostname}...";
        await Task.Delay(5000); // This is to simulate the eventual file-sending logic
        client.ViewModel.ResetStatus();
    }
}