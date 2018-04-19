using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Walterlv.ERMail.Views
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
