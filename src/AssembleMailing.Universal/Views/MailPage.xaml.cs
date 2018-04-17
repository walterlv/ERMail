using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;
using Walterlv.AssembleMailing.Utils;
using Walterlv.AssembleMailing.ViewModels;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class MailPage : Page
    {
        public MailPage()
        {
            InitializeComponent();
            _localFolder = ApplicationData.Current.LocalFolder.Path;
        }

        private readonly string _localFolder;

        public MailBoxViewModel ViewModel => (MailBoxViewModel) DataContext;

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (e.NewValue is MailBoxViewModel vm && vm.ConnectionInfo is MailBoxConnectionInfo info)
            {
                var cache = MailBoxCache.Get(_localFolder, info, PasswordManager.Current);
                var folders = await cache.LoadMailFoldersAsync();
                ViewModel.Folders.Clear();
                foreach (var folder in folders)
                {
                    ViewModel.Folders.Add(folder);
                }
            }
        }

        private async void MailFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailBoxFolderViewModel vm)) return;

            MailListView.DataContext = vm;
            var cache = MailBoxCache.Get(_localFolder, ViewModel.ConnectionInfo, PasswordManager.Current);
            var summaries = await cache.LoadMailsAsync(vm);
            vm.Mails.Clear();
            foreach (var summary in summaries)
            {
                vm.Mails.Add(summary);
            }
        }

        private async void MailGroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailGroupViewModel vm)) return;

            var cache = MailBoxCache.Get(_localFolder, ViewModel.ConnectionInfo, PasswordManager.Current);
            var mailCache = await cache.LoadMailAsync(vm.MailIds.First());
            if (!string.IsNullOrWhiteSpace(mailCache.HtmlBody))
            {
                WebView.NavigateToString(mailCache.HtmlBody);
            }
        }
    }
}
