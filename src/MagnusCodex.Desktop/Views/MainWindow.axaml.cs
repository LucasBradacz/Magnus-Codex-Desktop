using Avalonia.Controls;
using MagnusCodex.Desktop.ViewModels;

namespace MagnusCodex.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel vm)
            await vm.Cartas.CarregarAsync();
    }
}
