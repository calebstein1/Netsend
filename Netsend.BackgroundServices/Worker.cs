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
            // If we haven't seen a specific machine in 5 pings, we remove it from the list.
            // This may need some tuning, as we may have issues with a busy network since we can only see one client per ping.
            _clientsToDelete = FoundClients.Where(c => c.PingCounter < _pingCounter - 5).ToList();
            foreach(var client in _clientsToDelete)
            {
                FoundClients.Remove(client);
            }

            NetworkDiscovery.BroadcastService();
            
            // Once we find another client on the network, we need to make sure that we haven't already discovered the client,
            // and that it's not just our local machine.
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
                // If we find a client we've already discovered, we'll just update the ping counter so we can track how
                // recently we've visited.
                // It's ok to use else here instead of filtering by isLocalMachine, because if foundClient is the local machine,
                // clientToUpdate will be null and so nothing will happen.
                var clientToUpdate = FoundClients.FirstOrDefault(c => Equals(c.Client.Address, foundClient.Address));
                if (clientToUpdate != null)
                    clientToUpdate.PingCounter = _pingCounter;
            }

            Console.WriteLine($"Operation {_pingCounter}: {FoundClients.Count} clients discovered");

            // Update the ping counter for the next go-around
            _pingCounter++;
            await Task.Delay(1000, stoppingToken);
        }
    }
}