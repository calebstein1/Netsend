using Netsend.Models;
using Netsend.Networking;

namespace Netsend.BackgroundServices;

public class Worker : BackgroundService
{
    public List<ClientInfo> FoundClients = [];
    private int pingCounter = 0;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            /*if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }*/

            FoundClients.RemoveAll(c => c.PingCounter < pingCounter - 5);
            
            NetworkDiscovery.BroadcastService();
            var foundClient = NetworkDiscovery.FindService();
            if (FoundClients.All(c => !Equals(c.Client.Address, foundClient.Address)))
            {
                FoundClients.Add(new ClientInfo(foundClient, pingCounter));
            }
            else
            {
                foreach (var client in FoundClients.Where(c => Equals(c.Client.Address, foundClient.Address)))
                {
                    client.PingCounter = pingCounter;
                }
            }

            Console.WriteLine($"Operation {pingCounter}: {FoundClients.Count} clients discovered");
            /*foreach (var client in FoundClients)
            {
                Console.WriteLine(client.Address);
            }
            Console.WriteLine("\n");*/

            pingCounter++;
            await Task.Delay(1000, stoppingToken);
        }
    }
}