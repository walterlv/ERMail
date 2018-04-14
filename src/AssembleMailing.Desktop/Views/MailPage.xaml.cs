using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Walterlv.AssembleMailing.Views
{
    public class MailPage : UserControl
    {
        public MailPage()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
