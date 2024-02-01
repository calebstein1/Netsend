namespace Netsend.Models

open System.Net

type FoundClient = {
    Address : IPAddress
    Hostname : string
    OS : string
}

type FileRequest = {
    Destination : IPEndPoint
    File : string
    FileChecksum : string
}

type IFoundClientExt =
    abstract member Client : FoundClient with get