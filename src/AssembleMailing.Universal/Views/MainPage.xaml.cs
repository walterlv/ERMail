#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
#endif

namespace Walterlv.AssembleMailing.Views
{
#if WINDOWS_UWP
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // VisualStateManager.GoToState(MenuButton, "OverflowWithMenuIcons", false);
        }
#else
    public class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
        }
#endif

    }
}
