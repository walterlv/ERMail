using System;
using System.IO;
using System.Linq;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ConfigMailBoxDialog : ContentDialog
    {
        private readonly MailBoxConfigurationFile _configurationFile;

        public ConfigMailBoxDialog()
        {
            InitializeComponent();

            var localFolder = ApplicationData.Current.LocalFolder;
            _configurationFile = new MailBoxConfigurationFile(
                Path.Combine(localFolder.Path, "MailBoxConfiguration.json"));
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var configuration = await _configurationFile.ReadAsync();
            ConnectionInfo = configuration.Connections.FirstOrDefault() ?? new MailBoxConnectionInfo();
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

            var configuration = new MailBoxConfiguration();
            configuration.Connections.Add(ConnectionInfo);
            await _configurationFile.SaveAsync(configuration);

            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
        }

        private const string MailVaultResourceName = "Walterlv.AssembleMailing";
    }
}
