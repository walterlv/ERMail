using Avalonia;
using Avalonia.Logging.Serilog;
using Walterlv.AssembleMailing.ViewModels;
using Walterlv.AssembleMailing.Views;

namespace Walterlv.AssembleMailing
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
