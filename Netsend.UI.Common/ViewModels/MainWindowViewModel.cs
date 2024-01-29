using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Netsend.BackgroundServices;
using Netsend.Models;
using Netsend.UI.Common.Services;
using ReactiveUI;

namespace Netsend.UI.Common.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        FoundClientsList = new FoundClientsListViewModel();
    }
    
    public FoundClientsListViewModel FoundClientsList { get; }
}