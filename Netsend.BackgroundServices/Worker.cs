using System.Collections.ObjectModel;
using System.Net;
using Netsend.Models;
using Netsend.Networking;

namespace Netsend.BackgroundServices;

public class Worker : BackgroundService
{
    public static ObservableCollection<ClientInfo> FoundClients { get; } = [];
    private List<ClientInfo> _clientsToDelete = [];
    private int _pingCounter;
    private IPAddress[] _localIPs = Dns.GetHostAddresses(Dns.GetHostName());
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _clientsToDelete = FoundClients.Where(c => c.PingCounter < _pingCounter - 5).ToList();
            foreach(var client in _clientsToDelete)
            {
                FoundClients.Remove(client);
            }

            NetworkDiscovery.BroadcastService();
            var foundClient = NetworkDiscovery.FindService();
            var alreadyDiscovered = FoundClients.Any(c => c.Client.Address.Equals(foundClient.Address) ||
                                                                      c.Client.Hostname.Equals(foundClient.Hostname));
            var isLocalMachine = foundClient.Hostname.Equals(Dns.GetHostName()) ||
                                 _localIPs.Any(ip => ip.Equals(foundClient.Address));
            
            if (!alreadyDiscovered && !isLocalMachine)
            {
                FoundClients.Add(new ClientInfo(foundClient, _pingCounter));
            }
            else
            {
                var clientToUpdate = FoundClients.FirstOrDefault(c => Equals(c.Client.Address, foundClient.Address));
                if (clientToUpdate != null)
                    clientToUpdate.PingCounter = _pingCounter;
            }

            Console.WriteLine($"Operation {_pingCounter}: {FoundClients.Count} clients discovered");

            _pingCounter++;
            await Task.Delay(1000, stoppingToken);
        }
    }
}