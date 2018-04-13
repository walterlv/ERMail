using System.Collections.ObjectModel;

namespace Walterlv.AssembleMailing.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Hello World!";

        public ObservableCollection<MailBoxViewModel> MailBoxes { get; } = new ObservableCollection<MailBoxViewModel>
        {
            new MailBoxViewModel {DisplayName = "Outlook"},
            new MailBoxViewModel {DisplayName = "Gmail"},
            new MailBoxViewModel {DisplayName = "iCloud"},
        };
    }
}
