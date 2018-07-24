using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Walterlv.ERMail.Mailing;
using Walterlv.ERMail.Models;
using Walterlv.ERMail.Utils;
using Walterlv.ERMail.ViewModels;
using MUXC = Microsoft.UI.Xaml.Controls;

namespace Walterlv.ERMail.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var localFolder = ApplicationData.Current.LocalFolder;
            _configurationFile = new FileSerializor<MailBoxConfiguration>(
                Path.Combine(localFolder.Path, "MailBoxConfiguration.json"));
        }

        private readonly FileSerializor<MailBoxConfiguration> _configurationFile;

        private MainViewModel Main => (MainViewModel) DataContext;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var configuration = await _configurationFile.ReadAsync();
            var storedInfo = configuration.Connections.FirstOrDefault();
            storedInfo = storedInfo ?? await ConfigConnectionInfo();
            var mailBox = new MailBoxViewModel
            {
                DisplayName = storedInfo.AccountName,
                MailAddress = storedInfo.Address,
                ConnectionInfo = storedInfo,
            };
            Main.MailBoxes.Insert(0, mailBox);
            MailBoxComboBox.SelectedIndex = 0;
            Main.CurrentMailBox = mailBox;
        }

        private async Task<MailBoxConnectionInfo> ConfigConnectionInfo(string address = null)
        {
            var configuration = await _configurationFile.ReadAsync();
            var connections = configuration.Connections;
            var connectionInfo = connections.FirstOrDefault(x => x.Address == address) ?? new MailBoxConnectionInfo();
            if (!string.IsNullOrWhiteSpace(connectionInfo.Address))
            {
                connectionInfo.Password = PasswordManager.Current.Retrieve(connectionInfo.Address);
            }

            var config = new ConfigMailBoxDialog(connectionInfo);
            var result = await config.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                PasswordManager.Current.Add(connectionInfo.Address, connectionInfo.Password);

                connections.Clear();
                connections.Add(connectionInfo);
                await _configurationFile.SaveAsync(configuration);
                return connectionInfo;
            }

            return null;
        }

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

        private async void MailBoxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = e.AddedItems.FirstOrDefault();
            if (selected is MailBoxViewModel vm && vm.ConnectionInfo is MailBoxConnectionInfo info)
            {
                var localFolder = ApplicationData.Current.LocalFolder.Path;
                MailCache = MailBoxCache.Get(localFolder, info, PasswordManager.Current);
                var folders = await MailCache.LoadMailFoldersAsync();
                Main.CurrentMailBox.Folders.Clear();
                foreach (var folder in folders)
                {
                    Main.CurrentMailBox.Folders.Add(folder);
                }

                var inbox = Main.CurrentMailBox.Folders.FirstOrDefault(x => x.FullName == "INBOX");
                MainFolderListView.SelectedItem = inbox;
            }
        }

        private async void MainFolderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Main.CurrentMailBox.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailBoxFolderViewModel vm)) return;

            Main.CurrentMailBox.CurrentFolder = vm;
            var summaries = await MailCache.LoadMailsAsync(vm);
            vm.Mails.Clear();
            foreach (var summary in summaries)
            {
                vm.Mails.Add(summary);
            }
        }

        private async void MailGroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebView.Navigate(new Uri("about:blank"));
            if (Main.CurrentMailBox.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailGroupViewModel vm)) return;

            var mailCache = await MailCache.LoadMailAsync(Main.CurrentMailBox.CurrentFolder, vm.MailIds.First());
            var file = await StorageFile.GetFileFromPathAsync(mailCache.HtmlFileName);
            var text = await FileIO.ReadTextAsync(file);
            WebView.NavigateToString(text);
        }

        private MailBoxCache _mailCache;
    }
}
