using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Walterlv.ERMail.Models;
using Walterlv.ERMail.Utils;
using Walterlv.ERMail.ViewModels;

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

        private MainViewModel ViewModel => (MainViewModel) DataContext;

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
            ViewModel.MailBoxes.Insert(0, mailBox);
            MailBoxListView.SelectedIndex = 0;
            ViewModel.CurrentMailBox = mailBox;

            DetailFrame.Navigate(typeof(MailPage));
        }

        private async void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = await _configurationFile.ReadAsync();
            var address = configuration.Connections.Select(x => x.Address).FirstOrDefault();
            var info = await ConfigConnectionInfo(address);
            if (info != null)
            {
                ViewModel.MailBoxes[0].ConnectionInfo = info;
                ViewModel.MailBoxes[0].DisplayName = info.AccountName;
            }
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
    }
}
