using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsend.UI.Common.ViewModels;
using Netsend.UI.Common.Views;
using Netsend.BackgroundServices;
using Netsend.Networking;

namespace Netsend.UI.Common;

public partial class App : Application
{
    private IHost? _host;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            })
            .Build();
        Task.Run(() => _host.StartAsync());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            desktop.Exit += async (sender, e) =>
            {
                await _host.StopAsync();
                _host.Dispose();
                NetworkDiscovery.ShutdownService();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}