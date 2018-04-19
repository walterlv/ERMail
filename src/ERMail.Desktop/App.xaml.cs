using Avalonia;
using Avalonia.Markup.Xaml;

namespace Walterlv.ERMail
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
