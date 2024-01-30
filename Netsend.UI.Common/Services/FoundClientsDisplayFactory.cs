using Avalonia.Media.Imaging;
using Netsend.Models;
using Netsend.UI.Common.DataModel;
using Netsend.UI.Common.ViewModels;

namespace Netsend.UI.Common.Services;

public class FoundClientsDisplayFactory(FoundClientsListViewModel viewModel)
{
    public FoundClientDisplay Create(FoundClient client, Bitmap icon)
    {
        return new FoundClientDisplay(client, icon, viewModel);
    }
}