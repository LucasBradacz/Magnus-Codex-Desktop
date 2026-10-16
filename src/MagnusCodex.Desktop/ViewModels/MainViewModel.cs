using CommunityToolkit.Mvvm.ComponentModel;

namespace MagnusCodex.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public CartasViewModel Cartas { get; } = new();
}
