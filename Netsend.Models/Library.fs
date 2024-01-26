namespace Netsend.Models

open System.Net

type FoundClient = {
    Address: IPAddress
    Hostname: string
    OS: string
}

type FoundClientDisplay = {
    Hostname: string
    OS: string
}