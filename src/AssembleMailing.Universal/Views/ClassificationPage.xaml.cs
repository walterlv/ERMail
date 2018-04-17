using System;
using System.Collections.Async;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Walterlv.AssembleMailing.Classification;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;
using Walterlv.AssembleMailing.Utils;
using Walterlv.AssembleMailing.ViewModels;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ClassificationPage : Page, ILogger
    {
        public ClassificationPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
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

            if (e.Parameter is ValueTuple<MailBoxConnectionInfo, MailBoxFolderViewModel, MailGroupViewModel> tuple)
            {
                var (info, folder, group) = tuple;

                var localFolder = ApplicationData.Current.LocalFolder.Path;
                var mailCache = MailBoxCache.Get(localFolder, info, PasswordManager.Current);

                var current = await mailCache.LoadMailAsync(folder, group.MailIds.First());
                var mails = mailCache.EnumerateMailsAsync(folder);

                await new NaiveBayesClassifier(this).RunAsync(current.Content, mails.Select(x => x.Content));
            }
        }

        private ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        async void ILogger.Log(string message, string callerName)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                message = $"[{DateTimeOffset.Now:T}] [{callerName}] {message}";
                Logs.Add(message);
            });
        }
    }
}
