using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using ReactiveUI;
using Walterlv.AssembleMailing.Models;

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

        public MailBoxConnectionInfo ConnectionInfo
        {
            get => _connectionInfo;
            set => this.RaiseAndSetIfChanged(ref _connectionInfo, value);
        }

        public ObservableCollection<MailBoxFolderViewModel> Folders { get; }
            = new ObservableCollection<MailBoxFolderViewModel>();

        public MailBoxFolderViewModel CurrentFolder
        {
            get => _currentFolder;
            set => this.RaiseAndSetIfChanged(ref _currentFolder, value);
        }

        [ContractPublicPropertyName(nameof(DisplayName))]
        private string _displayName;

        [ContractPublicPropertyName(nameof(MailAddress))]
        private string _mailAddress;

        [ContractPublicPropertyName(nameof(CurrentFolder))]
        private MailBoxFolderViewModel _currentFolder;

        [ContractPublicPropertyName(nameof(ConnectionInfo))]
        private MailBoxConnectionInfo _connectionInfo;
    }
}
