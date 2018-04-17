using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ClassificationPage : Page
    {
        public ClassificationPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = e.Parameter;
            base.OnNavigatedTo(e);

            //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            //    Frame.CanGoBack ?
            //        AppViewBackButtonVisibility.Visible :
            //        AppViewBackButtonVisibility.Collapsed;

            //void OnBackRequested(object sender, BackRequestedEventArgs e)
            //{
            //    Frame.GoBack(new SlideNavigationTransitionInfo());
            //}
        }


        private ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        private void Log(string message, [CallerMemberName] string callerName = null)
        {
            message = $"[{DateTimeOffset.Now:T}] [{callerName}] {message}";
            Logs.Add(message);
            LogListView.ScrollIntoView(message);
        }
    }
}
