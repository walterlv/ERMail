using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MailKit;
using MimeKit;
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
            this.InitializeComponent();
        }

        public MailBoxViewModel ViewModel => (MailBoxViewModel) DataContext;

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (e.NewValue is MailBoxViewModel vm && vm.ConnectionInfo is MailBoxConnectionInfo info)
            {
                await FetchFoldersAsync(info, vm);
            }
        }

        private async void MailFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailBoxFolderViewModel vm)) return;

            MailListView.DataContext = vm;
            await FetchMailsAsync(ViewModel.ConnectionInfo, vm);
        }

        private async void MailGroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailGroupViewModel vm)) return;

            var body = await DownloadMailAsync(ViewModel.ConnectionInfo, vm);
            if (!string.IsNullOrWhiteSpace(body))
            {
                WebView.NavigateToString(body);
            }
        }

        private static async Task FetchFoldersAsync(MailBoxConnectionInfo info, MailBoxViewModel viewModel)
        {
            if (string.IsNullOrEmpty(info.Password))
            {
                FillPassword(info);
            }

            viewModel.Folders.Clear();
            using (var client = await new IncomingMailClient(info).ConnectAsync())
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0]);
                foreach (var folder in new[] {inbox}.Union(folders))
                {
                    viewModel.Folders.Add(new MailBoxFolderViewModel
                    {
                        Name = folder.Name,
                        Separator = folder.DirectorySeparator,
                        FullName = folder.FullName,
                    });
                }

                viewModel.CurrentFolder = viewModel.Folders[0];
            }
        }

        private static async Task FetchMailsAsync(MailBoxConnectionInfo info, MailBoxFolderViewModel viewModel)
        {
            using (var client = await new IncomingMailClient(info).ConnectAsync())
            {
                var folder = await client.GetFolderAsync(viewModel.FullName);
                folder.Open(FolderAccess.ReadOnly);

                viewModel.Mails.Clear();
                var messageSummaries = await folder.FetchAsync(folder.Count - 20, folder.Count - 1,
                    MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
                foreach (var summary in messageSummaries.Reverse())
                {
                    TextPart body;
                    try
                    {
                        body = (TextPart) await folder.GetBodyPartAsync(summary.UniqueId, summary.TextBody);
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
                    viewModel.Mails.Add(mailGroup);
                }

                viewModel.Mails.Add(new MailGroupViewModel());
            }
        }

        private static async Task<string> DownloadMailAsync(MailBoxConnectionInfo info, MailGroupViewModel mailGroupViewModel)
        {
            using (var client = await new IncomingMailClient(info).ConnectAsync())
            {
                client.Inbox.Open(FolderAccess.ReadOnly);
                try
                {
                    var message = await client.Inbox.GetMessageAsync(new UniqueId(mailGroupViewModel.MailIds.First()));
                    var htmlBody = message.HtmlBody;
                    return htmlBody;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private static void FillPassword(MailBoxConnectionInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Address))
            {
                info.Password = PasswordManager.Retrieve(info.Address);
            }
        }
    }
}
