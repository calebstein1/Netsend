using Netsend.Models;

namespace Netsend.BackgroundServices;

public class ClientInfo(FoundClient client, int initCounter) : IFoundClientExt
{
    public FoundClient Client { get; } = client;
    public int PingCounter { get; set; } = initCounter;
}