using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MagnusCodex.Data;
using MagnusCodex.Desktop.ViewModels;
using MagnusCodex.Desktop.Views;

namespace MagnusCodex.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Inicializa banco e seed antes de abrir qualquer janela.
            // O CSV está em Data/ ao lado do executável.
            var caminhoCsv = Path.Combine(
                AppContext.BaseDirectory, "Data", "magnus_codex_cartas.csv");

            // Roda de forma síncrona no startup — ok para operação local/rápida.
            DatabaseInitializer.InicializarAsync(caminhoCsv)
                .GetAwaiter().GetResult();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
