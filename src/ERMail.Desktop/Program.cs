using Avalonia;
using Avalonia.Logging.Serilog;
using Walterlv.ERMail.ViewModels;
using Walterlv.ERMail.Views;

namespace Walterlv.ERMail
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
