using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Netsend.Networking;

public class ObservableString : INotifyPropertyChanged
{
    private string? _value;

    public string? Value
    {
        get => _value;
        set
        {
            if (Equals(_value, value)) return;
            _value = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}