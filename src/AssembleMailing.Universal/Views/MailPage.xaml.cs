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
        }

        private MailBoxCache _mailCache;

        private MailBoxCache MailCache
        {
            get => _mailCache;
            set
            {
                if (Equals(_mailCache, value)) return;

                if (_mailCache is MailBoxCache oldValue)
                {
                    // Unregister cache events.
                }

                _mailCache = value;

                if (value != null)
                {
                    // Register cache events.
                }
            }
        }

        public MailBoxViewModel ViewModel => (MailBoxViewModel) DataContext;

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (e.NewValue is MailBoxViewModel vm && vm.ConnectionInfo is MailBoxConnectionInfo info)
            {
                var localFolder = ApplicationData.Current.LocalFolder.Path;
                MailCache = MailBoxCache.Get(localFolder, info, PasswordManager.Current);
                var folders = await MailCache.LoadMailFoldersAsync();
                ViewModel.Folders.Clear();
                foreach (var folder in folders)
                {
                    ViewModel.Folders.Add(folder);
                }

                var inbox = ViewModel.Folders.FirstOrDefault(x=>x.FullName == "INBOX");
                MailFolderComboBox.SelectedItem = inbox;
            }
        }

        private async void MailFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailBoxFolderViewModel vm)) return;

            MailListView.DataContext = vm;
            ViewModel.CurrentFolder = vm;
            var summaries = await MailCache.LoadMailsAsync(vm);
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

            var mailCache = await MailCache.LoadMailAsync(ViewModel.CurrentFolder, vm.MailIds.First());
            if (!string.IsNullOrWhiteSpace(mailCache?.HtmlBody))
            {
                WebView.NavigateToString(mailCache.HtmlBody);
            }
        }
    }
}
