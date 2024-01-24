namespace Netsend.Models

open System.Net

type FoundClient = {
    BroadcastMessage: string
    SourceAddress: IPAddress
}