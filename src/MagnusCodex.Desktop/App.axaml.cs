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
            var caminhoCsv = Path.Combine(
                AppContext.BaseDirectory, "Data", "magnus_codex_cartas.csv");

            var resultado = DatabaseInitializer.InicializarAsync(caminhoCsv)
                .GetAwaiter().GetResult();

            // Log de startup — visível no terminal ao rodar com "dotnet run"
            Console.WriteLine($"[DB] {resultado.CaminhoDb}");

            if (resultado.MigracoesAplicadas.Count > 0)
                Console.WriteLine($"[DB] Migrations aplicadas: {string.Join(", ", resultado.MigracoesAplicadas)}");
            else
                Console.WriteLine("[DB] Banco já atualizado, nenhuma migration necessária.");

            if (resultado.SeedRodou)
                Console.WriteLine($"[DB] Seed concluído: {resultado.TotalCartas} cartas importadas.");
            else
                Console.WriteLine($"[DB] Banco já populado: {resultado.TotalCartas} cartas no banco.");

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
