using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // VisualStateManager.GoToState(MenuButton, "OverflowWithMenuIcons", false);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var config = new ConfigMailBoxDialog();
            await config.ShowAsync();
        }
    }
}
