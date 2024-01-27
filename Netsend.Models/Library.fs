namespace Netsend.Models

open System.Net

type FoundClient = {
    Address : IPAddress
    Hostname : string
    OS : string
}

type IFoundClientExt =
    abstract member Client : FoundClient with get