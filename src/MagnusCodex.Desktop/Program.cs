using Avalonia;
using System;

namespace MagnusCodex.Desktop;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Captura exceções não tratadas em qualquer thread
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            Console.WriteLine("[ERRO FATAL] " + e.ExceptionObject);
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERRO STARTUP] " + ex);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
