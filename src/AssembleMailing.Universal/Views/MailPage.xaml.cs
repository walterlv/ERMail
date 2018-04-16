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
            if ((e.NewValue as MailBoxViewModel)?.ConnectionInfo is MailBoxConnectionInfo info)
            {
                await FetchMailsAsync(info);
            }
        }

        private async void MailGroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailGroupViewModel vm)) return;

            using (var client = await new IncomingMailClient(ViewModel.ConnectionInfo).ConnectAsync())
            {
                client.Inbox.Open(FolderAccess.ReadOnly);
                try
                {
                    var message = await client.Inbox.GetMessageAsync(new UniqueId(vm.MailIds.First()));
                    var htmlBody = message.HtmlBody;
                    WebView.NavigateToString(htmlBody);
                }
                catch (Exception ex)
                {
                }
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

                var folder = ViewModel.CurrentFolder ?? new MailBoxFolderViewModel();
                folder.Mails.Clear();
                ViewModel.CurrentFolder = folder;
                var messageSummaries = await inbox.FetchAsync(inbox.Count - 20, inbox.Count - 1,
                    MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
                foreach (var summary in messageSummaries.Reverse())
                {
                    TextPart body;
                    try
                    {
                        body = (TextPart)await inbox.GetBodyPartAsync(summary.UniqueId, summary.TextBody);
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

        private static void FillPassword(MailBoxConnectionInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Address))
            {
                info.Password = PasswordManager.Retrieve(info.Address);
            }
        }
    }
}
