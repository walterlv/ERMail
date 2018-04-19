using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Walterlv.ERMail.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }

    }
}
