using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Netsend.Models;
using Netsend.UI.Common.ViewModels;
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
    public ReactiveCommand<FoundClientDisplay, Task> SendDirectoryCommand { get; } =
        ReactiveCommand.Create<FoundClientDisplay, Task>(SendDirectoryAsync);
    
    private static async Task SendFileAsync(FoundClientDisplay client)
    {
        client.ViewModel.Status = $"Sending file to {client.Client.Hostname}...";
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