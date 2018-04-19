using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using Newtonsoft.Json;

namespace Walterlv.ERMail.Models
{
    /// <summary>
    /// Stores connection information of a specified mail box.
    /// This is a model that will be serialized into a file.
    /// </summary>
    public class MailBoxConnectionInfo
    {
        public string Address
        {
            get => _address;
            set => SetAndValidate(ref _address, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetAndValidate(ref _userName, value);
        }

        [JsonIgnore]
        public string Password
        {
            get => _password;
            set => SetAndValidate(ref _password, value);
        }

        public string AccountName
        {
            get => _accountName;
            set => SetAndValidate(ref _accountName, value);
        }

        public string EnvelopeName
        {
            get => _envelopeName;
            set => SetAndValidate(ref _envelopeName, value);
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

        [JsonIgnore] public string IncomingServerHost => _incomingServerHost;
        [JsonIgnore] public int IncomingServerPort => _incomingServerPort;
        [JsonIgnore] public string OutgoingServerHost => _outgoingServerHost;
        [JsonIgnore] public int OutgoingServerPort => _outgoingServerPort;

        private static void SetHostPort(out string hostField, out int portField, string value)
        {
            var parts = value.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
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

        public bool Validate()
        {
            var invalid = string.IsNullOrWhiteSpace(Address)
                          || string.IsNullOrWhiteSpace(UserName)
                          || string.IsNullOrEmpty(Password)
                          || string.IsNullOrWhiteSpace(AccountName)
                          || string.IsNullOrWhiteSpace(EnvelopeName)
                          || string.IsNullOrWhiteSpace(_incomingServerHost)
                          || string.IsNullOrWhiteSpace(_outgoingServerHost);
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

        [ContractPublicPropertyName(nameof(IncomingServerHost))]
        private string _incomingServerHost;

        [ContractPublicPropertyName(nameof(IncomingServerPort))]
        private int _incomingServerPort;

        [ContractPublicPropertyName(nameof(OutgoingServerHost))]
        private string _outgoingServerHost;

        [ContractPublicPropertyName(nameof(OutgoingServerPort))]
        private int _outgoingServerPort;
    }
}
