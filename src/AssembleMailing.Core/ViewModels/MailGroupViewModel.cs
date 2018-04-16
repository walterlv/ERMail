using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using ReactiveUI;

namespace Walterlv.AssembleMailing.ViewModels
{
    public class MailGroupViewModel : ViewModelBase
    {
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Topic
        {
            get => _topic;
            set => this.RaiseAndSetIfChanged(ref _topic, value);
        }

        public string Excerpt
        {
            get => _excerpt;
            set => this.RaiseAndSetIfChanged(ref _excerpt, value);
        }

        public ObservableCollection<uint> MailIds { get; } = new ObservableCollection<uint>();

        [ContractPublicPropertyName(nameof(Title))]
        private string _title;

        [ContractPublicPropertyName(nameof(Topic))]
        private string _topic;

        [ContractPublicPropertyName(nameof(Excerpt))]
        private string _excerpt;
    }
}
