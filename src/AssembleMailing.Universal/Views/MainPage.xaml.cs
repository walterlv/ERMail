using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Walterlv.AssembleMailing.Mailing;
using Walterlv.AssembleMailing.Models;

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var configuration = await _configurationFile.ReadAsync();
            var address = configuration.Connections.Select(x => x.Address).FirstOrDefault();
            await ConfigConnectionInfo(address);
        }

        private async void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = await _configurationFile.ReadAsync();
            var address = configuration.Connections.Select(x => x.Address).FirstOrDefault();
            await ConfigConnectionInfo(address);
        }

        private async Task ConfigConnectionInfo(string address = null)
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
            }
        }
    }
}
