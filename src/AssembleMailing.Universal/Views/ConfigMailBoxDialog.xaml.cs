using System;
using Windows.UI.Xaml.Controls;
using MailKit.Security;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ConfigMailBoxDialog : ContentDialog
    {
        public ConfigMailBoxDialog(MailBoxConnectionInfo connectionInfo)
        {
            InitializeComponent();
            ConnectionInfo = connectionInfo;
        }

        public MailBoxConnectionInfo ConnectionInfo
        {
            get => (MailBoxConnectionInfo) DataContext;
            set => DataContext = value;
        }

        private string ErrorTip { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
            var info = ConnectionInfo;
            if (!info.Validate()) return;
            var deferral = e.GetDeferral();

            var client = new IncomingMailClient(info);

            try
            {
                await client.TestConnectionAsync();
                deferral.Complete();
            }
            catch (AuthenticationException ex)
            {
                e.Cancel = true;
                ErrorTip = ex.Message;
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                ErrorTip = ex.Message;
            }
        }
    }
}
