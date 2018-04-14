using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using ReactiveUI;

namespace Walterlv.AssembleMailing.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";

        public ObservableCollection<MailBoxViewModel> MailBoxes { get; } = new ObservableCollection<MailBoxViewModel>
        {
            new MailBoxViewModel {DisplayName = "Outlook", CurrentFolder = new MailBoxFolderViewModel()},
            new MailBoxViewModel {DisplayName = "Gmail", CurrentFolder = new MailBoxFolderViewModel()},
            new MailBoxViewModel {DisplayName = "iCloud", CurrentFolder = new MailBoxFolderViewModel()},
        };

        public MailBoxViewModel CurrentMailBox
        {
            get => _currentMailBox ?? (_currentMailBox = MailBoxes[1]);
            set => this.RaiseAndSetIfChanged(ref _currentMailBox, value);
        }

        [ContractPublicPropertyName(nameof(CurrentMailBox))]
        private MailBoxViewModel _currentMailBox;
    }
}
