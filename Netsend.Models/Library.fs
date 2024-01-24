namespace Netsend.Models

open System.Net

type FoundClient = {
    Hostname: string
    Address: IPAddress
}