# Netsend

Netsend is a cross-platform clone of Apple's AirDrop, written in .Net

## Purpose

This is meant as an exercise for me to learn .Net in the context of a more complex project.
It's all still very early on in development, so don't expect anything super functional at this point.

## Architecture

The core functionality lives in Netsend.BackgroundServices.
This assembly handles the main program loop, keeps track of network connections, and creates needed objects that get sent wherever they need to go.
Think of it as the controller in an MVC web app paradigm; it's taking external requests, outsourcing the logic out to other libraries, and sending what's needed to the UI.

Netsend.Networking is the library that handles the actual logic of the server broadcast and discovery over UDP, as well as establishing the direct connection with the peer over TCP.
Currently, only the UDP functionality is implemented, the TCP bits are in development.

Netsend.Models defines the data structures used by other libraries.
This library is written in F# for its cleaner syntax, type safety, and immutability.

The UI is contained in Netsend.UI.Common.
This is an AvaloniaUI project that uses the MVVM architecture to recieve and display data from the BackgroundServices worker, as well as control its functionality.
Ultimately there will be additional UI libraries (Netsend.UI.Windows, Netsend.UI.Linux, Netsend.UI.Mac) which will implement platform-specific bits such as a system tray icon and maybe file explorer integration, but this is all stuff to be added at the end.

## Progress

Currently, the app runs, advertises itself on the local network over UDP, and will discover other instances of the app running on the local network.
Other running app clients on the network are displayed in the GUI with their hostnames and an OS-specific icon.
The send file button opens a file selector, then establishes a TCP connection with the selected client and gets a simple message back, containing the client's hostname.

## Building

Netsend depends on .NET 8, but besides that one dependency, all it should take is a clone of the repository and a `dotnet run` in the Netsend.UI.Common project to get it all going.
I've built and run the app on both Linux and Windows.
It builds but fails to run on MacOS at the moment, something about the part of Netsend.BackgroundServices that gets a list of the local machine IP addresses.
I'm not sure if this is some permissions issue with the OS blocking access, or if that specific function is just unavailable on MacOS, but either way, I'll have that issue buttoned up one way or another prior to release.
I don't think .NET supports BSD yet, but _if_ it does, and _if_ this builds there, the BSD-heads out there will have to live with the fact that they'll show up as a Linux icon to other clients :laughing:

## What's next?

Now that TCP is working, next is being able to send JSON messages back and forth, rather than just a simple text response.
Once that's working, that TCP connection will need to be used to transfer single files.
After that, I'll be extending the file transfer to allow for full directories.
