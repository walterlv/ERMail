using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MailKit;
using MimeKit.Text;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;
using Walterlv.AssembleMailing.ViewModels;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var localFolder = ApplicationData.Current.LocalFolder;
            _configurationFile = new MailBoxConfigurationFile(
                Path.Combine(localFolder.Path, "MailBoxConfiguration.json"));
        }

        private readonly MailBoxConfigurationFile _configurationFile;
        private const string MailVaultResourceName = "Walterlv.AssembleMailing";

        private MainViewModel ViewModel => (MainViewModel) DataContext;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var configuration = await _configurationFile.ReadAsync();
            var storedInfo = configuration.Connections.FirstOrDefault();
            if (storedInfo != null)
            {
                var folderViewModel = new MailBoxFolderViewModel();
                folderViewModel.Mails.Clear();
                ViewModel.MailBoxes.Insert(0, new MailBoxViewModel
                {
                    DisplayName = storedInfo.AccountName,
                    CurrentFolder = folderViewModel,
                    MailAddress = storedInfo.Address,
                });
                MailBoxListView.SelectedIndex = 0;
                await FetchMailsAsync(storedInfo);
            }
            else
            {
                var info = await ConfigConnectionInfo();
                if (info != null)
                {
                    await FetchMailsAsync(info);
                }
            }
        }

        private async void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = await _configurationFile.ReadAsync();
            var address = configuration.Connections.Select(x => x.Address).FirstOrDefault();
            await ConfigConnectionInfo(address);
            var info = await ConfigConnectionInfo(address);
            if (info != null)
            {
                await FetchMailsAsync(info);
            }
        }

        private async Task FetchMailsAsync(MailBoxConnectionInfo info)
        {
            if (string.IsNullOrEmpty(info.Password))
            {
                FillPassword(info);
            }
            var client = new MailClient(info.UserName, info.Password,
                info.IncomingServerHost, info.IncomingServerPort > 0 ? info.IncomingServerPort : (int?) null);
            var mailClient = await client.ConnectAsync();
            var inbox = mailClient.Inbox;
            inbox.Open(FolderAccess.ReadOnly);

            var folder = ViewModel.CurrentMailBox.CurrentFolder;
            folder.Mails.Clear();
            for (var i = inbox.Count - 1; i > inbox.Count - 21; i--)
            {
                var message = await inbox.GetMessageAsync(i);
                folder.Mails.Add(new MailGroupViewModel
                {
                    Title = message.From.FirstOrDefault()?.Name ?? "(Anonymous)",
                    Topic = message.Subject,
                    Excerpt = message.GetTextBody(TextFormat.Plain),
                });
            }

            folder.Mails.Add(new MailGroupViewModel());
        }

        private async Task<MailBoxConnectionInfo> ConfigConnectionInfo(string address = null)
        {
            var configuration = await _configurationFile.ReadAsync();
            var connections = configuration.Connections;
            var connectionInfo = connections.FirstOrDefault(x => x.Address == address) ?? new MailBoxConnectionInfo();
            var vault = new PasswordVault();
            if (!string.IsNullOrWhiteSpace(connectionInfo.Address))
            {
                var credential = vault.Retrieve(MailVaultResourceName, connectionInfo.Address);
                connectionInfo.Password = credential.Password;
            }

            var config = new ConfigMailBoxDialog(connectionInfo);
            var result = await config.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                vault.Add(new PasswordCredential(
                    MailVaultResourceName, connectionInfo.Address, connectionInfo.Password));

                connections.Clear();
                connections.Add(connectionInfo);
                await _configurationFile.SaveAsync(configuration);
                return connectionInfo;
            }

            return null;
        }

        private static void FillPassword(MailBoxConnectionInfo info)
        {
            var vault = new PasswordVault();
            if (!string.IsNullOrWhiteSpace(info.Address))
            {
                var credential = vault.Retrieve(MailVaultResourceName, info.Address);
                info.Password = credential.Password;
            }
        }
    }
}
