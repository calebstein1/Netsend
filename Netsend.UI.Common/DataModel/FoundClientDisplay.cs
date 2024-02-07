using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Netsend.Models;
using Netsend.UI.Common.ViewModels;
using ReactiveUI;

namespace Netsend.UI.Common.DataModel;

public class FoundClientDisplay(FoundClient client, Bitmap iconPath, FoundClientsListViewModel vm) : IFoundClientExt
{
    public FoundClient Client { get; } = client;
    public Bitmap IconPath { get; } = iconPath;
    private FoundClientsListViewModel Vm { get; } = vm;

    // I really don't like having all this logic in the DataModel, but try as I might, I haven't been able to make anything
    // work as cleanly as this. It's unfortunate, but for now this is just how it has to be.
    public ReactiveCommand<FoundClientDisplay, Task> SendFileCommand { get; } =
        ReactiveCommand.Create<FoundClientDisplay, Task>(SendFileAsync);
    
    private static async Task SendFileAsync(FoundClientDisplay client)
    {
        client.Vm.Status = $"Sending file to {client.Client.Hostname}...";
        var storageProvider = new Window().StorageProvider;
        var file = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Netsend"
        });
        client.Vm.Status = $"You selected {file[0].Path}";
        await client.Vm.Tcp.SendRequestAsync(client.Client.Address, file[0].Path);
        await Task.Delay(5000);
        client.Vm.ResetStatus();
    }
}