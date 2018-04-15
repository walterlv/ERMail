using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Windows.UI.Xaml.Controls;
using Walterlv.AssembleMailing.Mailing;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Walterlv.AssembleMailing.Views
{
    public sealed partial class ConfigMailBoxDialog : ContentDialog
    {
        public ConfigMailBoxDialog()
        {
            this.InitializeComponent();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
            e.Cancel = true;
            if (!Validate()) return;
            var deferral = e.GetDeferral();

            var client = new MailClient(_incomingServerHost,
                _incomingServerPort > 0 ? _incomingServerPort : 993,
                UserName, Password);
            await client.ConnectAsync();
            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs e)
        {
        }

        public string Address
        {
            get => _address;
            private set => SetAndValidate(ref _address, value);
        }

        public string UserName
        {
            get => _userName;
            private set => SetAndValidate(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            private set => SetAndValidate(ref _password, value);
        }

        public string AccountName
        {
            get => _accountName;
            private set => SetAndValidate(ref _accountName, value);
        }

        public string EnvelopeName
        {
            get => _envelopeName;
            private set => SetAndValidate(ref _envelopeName, value);
        }

        public string IncomingServer
        {
            get => _incomingServerPort > 0 ? $"{_incomingServerHost}:{_incomingServerPort}" : _incomingServerHost;
            set => SetAndValidate(out _incomingServerHost, out _incomingServerPort, value);
        }

        public string OutgoingServer
        {
            get => _outgoingServerPort > 0 ? $"{_outgoingServerHost}:{_outgoingServerPort}" : _outgoingServerHost;
            set => SetAndValidate(out _outgoingServerHost, out _outgoingServerPort, value);
        }

        private static void SetHostPort(out string hostField, out int portField, string value)
        {
            var parts = value.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                hostField = parts[0].Trim();
                portField = 0;
            }
            else if (parts.Length == 2 &&
                     int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var port))
            {
                hostField = parts[0].Trim();
                portField = port;
            }
            else
            {
                hostField = null;
                portField = 0;
            }
        }

        private void SetAndValidate(out string hostField, out int portField, string value)
        {
            SetHostPort(out hostField, out portField, value);
            Validate();
        }

        private void SetAndValidate<T>(ref T field, T value)
        {
            if (Equals(field, value)) return;

            field = value;
            Validate();
        }

        private bool Validate()
        {
            var invalid = string.IsNullOrWhiteSpace(Address)
                          || string.IsNullOrWhiteSpace(UserName)
                          || string.IsNullOrEmpty(Password)
                          || string.IsNullOrWhiteSpace(AccountName)
                          || string.IsNullOrWhiteSpace(EnvelopeName)
                          || string.IsNullOrWhiteSpace(_incomingServerHost)
                          || string.IsNullOrWhiteSpace(_outgoingServerHost);
            SecondaryButtonText = invalid ? string.Empty : "Sign in";
            return !invalid;
        }

        [ContractPublicPropertyName(nameof(Address))]
        private string _address;

        [ContractPublicPropertyName(nameof(UserName))]
        private string _userName;

        [ContractPublicPropertyName(nameof(Password))]
        private string _password;

        [ContractPublicPropertyName(nameof(AccountName))]
        private string _accountName;

        [ContractPublicPropertyName(nameof(EnvelopeName))]
        private string _envelopeName;

        private string _incomingServerHost;
        private int _incomingServerPort;
        private string _outgoingServerHost;
        private int _outgoingServerPort;
    }
}
