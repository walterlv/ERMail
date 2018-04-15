using System;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ConfigMailBoxDialog : ContentDialog
    {
        public ConfigMailBoxDialog()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ConnectionInfo = new MailBoxConnectionInfo();
        }

        public MailBoxConnectionInfo ConnectionInfo { get; private set; }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
            e.Cancel = true;
            if (!ConnectionInfo.Validate()) return;
            var deferral = e.GetDeferral();

            var client = new MailClient(ConnectionInfo.IncomingServerHost,
                ConnectionInfo.IncomingServerPort > 0 ? ConnectionInfo.IncomingServerPort : 993,
                ConnectionInfo.UserName, ConnectionInfo.Password);
            await client.ConnectAsync();

            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(MailVaultResourceName, ConnectionInfo.Address, ConnectionInfo.Password));


            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
        }

        private const string MailVaultResourceName = "Walterlv.AssembleMailing";
    }
}
