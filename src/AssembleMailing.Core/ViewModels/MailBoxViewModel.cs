using System.Diagnostics.Contracts;
using ReactiveUI;

namespace Walterlv.AssembleMailing.ViewModels
{
    public sealed class MailBoxViewModel : ViewModelBase
    {
        public string DisplayName
        {
            get => _displayName;
            set => this.RaiseAndSetIfChanged(ref _displayName, value);
        }

        public string MailAddress
        {
            get => _mailAddress;
            set => this.RaiseAndSetIfChanged(ref _mailAddress, value);
        }

        [ContractPublicPropertyName(nameof(DisplayName))]
        private string _displayName;

        [ContractPublicPropertyName(nameof(MailAddress))]
        private string _mailAddress;
    }
}
