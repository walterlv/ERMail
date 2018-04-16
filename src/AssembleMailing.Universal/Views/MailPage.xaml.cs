using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using MailKit;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;
using Walterlv.AssembleMailing.ViewModels;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class MailPage : Page
    {
        public MailPage()
        {
            this.InitializeComponent();
        }

        public MailBoxConnectionInfo ConnectionInfo { get; set; }

        private async void MailGroupListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConnectionInfo is null) return;
            if (!(e.AddedItems.FirstOrDefault() is MailGroupViewModel vm)) return;

            using (var client = await new IncomingMailClient(ConnectionInfo).ConnectAsync())
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
    }
}
