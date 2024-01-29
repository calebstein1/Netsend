using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Netsend.Models;
using Netsend.UI.Common.ViewModels;
using ReactiveUI;

namespace Netsend.UI.Common;

public class FoundClientDisplay(FoundClient client, Bitmap iconPath, MainWindowViewModel viewModel) : IFoundClientExt
{
    public FoundClient Client { get; } = client;
    public Bitmap IconPath { get; } = iconPath;
    private MainWindowViewModel ViewModel { get; } = viewModel;

    public ReactiveCommand<FoundClientDisplay, Task> SendFileCommand { get; } =
        ReactiveCommand.Create<FoundClientDisplay, Task>(SendFileAsync);
    
    private static async Task SendFileAsync(FoundClientDisplay client)
    {
        client.ViewModel.Status = $"Sending file to {client.Client.Hostname}...";
        await Task.Delay(5000); // This is to simulate the eventual file-sending logic
        client.ViewModel.ResetStatus();
    }
}