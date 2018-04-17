using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using ReactiveUI;
using Walterlv.AssembleMailing.Models;

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

        public static implicit operator MailGroupViewModel(MailSummary source)
        {
            var target = new MailGroupViewModel
            {
                Title = source.Title,
                Topic = source.Topic,
                Excerpt = source.Excerpt,
            };
            foreach (var mailId in source.MailIds)
            {
                target.MailIds.Add(mailId);
            }

            return target;
        }

        public static implicit operator MailSummary(MailGroupViewModel source)
        {
            var target = new MailSummary
            {
                Title = source.Title,
                Topic = source.Topic,
                Excerpt = source.Excerpt,
            };
            foreach (var mailId in source.MailIds)
            {
                target.MailIds.Add(mailId);
            }

            return target;
        }

        [ContractPublicPropertyName(nameof(Title))]
        private string _title;

        [ContractPublicPropertyName(nameof(Topic))]
        private string _topic;

        [ContractPublicPropertyName(nameof(Excerpt))]
        private string _excerpt;
    }
}
