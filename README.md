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
Other running instances of the app on the network are displayed in the CLI for now, GUI display is in progress.

## What's next?

Next on the to-do is to get two running instances paired up over a TCP connection, and be able to send JSON messages back and forth.
Once that's working, that TCP connection will need to be used to transfer single files.
After that, I'll be extending the file transfer to allow for full directories.
