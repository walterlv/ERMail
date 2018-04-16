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
using MimeKit;
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
                MailPage.ConnectionInfo = storedInfo;
                await FetchMailsAsync(storedInfo);
            }
            else
            {
                var info = await ConfigConnectionInfo();
                if (info != null)
                {
                    MailPage.ConnectionInfo = info;
                    await FetchMailsAsync(info);
                }
            }
        }

        private async void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = await _configurationFile.ReadAsync();
            var address = configuration.Connections.Select(x => x.Address).FirstOrDefault();
            var info = await ConfigConnectionInfo(address);
            if (info != null)
            {
                MailPage.ConnectionInfo = info;
                ViewModel.MailBoxes[0].DisplayName = info.AccountName;
                await FetchMailsAsync(info);
            }
        }

        private async Task FetchMailsAsync(MailBoxConnectionInfo info)
        {
            if (string.IsNullOrEmpty(info.Password))
            {
                FillPassword(info);
            }
            using (var client = await new IncomingMailClient(info).ConnectAsync())
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var folder = ViewModel.CurrentMailBox.CurrentFolder;
                folder.Mails.Clear();
                var messageSummaries = await inbox.FetchAsync(inbox.Count - 20, inbox.Count - 1,
                    MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
                foreach (var summary in messageSummaries.Reverse())
                {
                    TextPart body;
                    try
                    {
                        body = (TextPart) await inbox.GetBodyPartAsync(summary.UniqueId, summary.TextBody);
                    }
                    catch (Exception ex)
                    {
                        body = null;
                    }

                    var mailGroup = new MailGroupViewModel
                    {
                        Title = summary.Envelope.From.Select(x => x.Name).FirstOrDefault() ?? "(Anonymous)",
                        Topic = summary.Envelope.Subject,
                        Excerpt = body?.Text?.Replace(Environment.NewLine, " "),
                    };
                    mailGroup.MailIds.Add(summary.UniqueId.Id);
                    folder.Mails.Add(mailGroup);
                }

                folder.Mails.Add(new MailGroupViewModel());
            }
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
