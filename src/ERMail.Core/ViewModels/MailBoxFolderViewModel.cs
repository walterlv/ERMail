using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using ReactiveUI;
using Walterlv.ERMail.Models;

namespace Walterlv.ERMail.ViewModels
{
    public class MailBoxFolderViewModel : ViewModelBase
    {
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }

        public char Separator
        {
            get => _separator;
            set => this.RaiseAndSetIfChanged(ref _separator, value);
        }

        public ObservableCollection<MailGroupViewModel> Mails { get; } = new ObservableCollection<MailGroupViewModel>
        {
            new MailGroupViewModel
            {
                Title = "Google Analytics",
                Topic = "[Action Required] Important updates on Google Analytics DataRetention and the General Data Protection Regulation (GDPR)",
                Excerpt = "Dear Google Analytics Administrator, Over the past year we've shared how we are preparing to meet the requirements of the GDPR, the new data protection law coming into force on May 25, 2018.",
            },
            new MailGroupViewModel
            {
                Title = "Twitter",
                Topic = "lindexi is reportedly considering asking a new question.",
                Excerpt = "34 events",
            },
            new MailGroupViewModel
            {
                Title = "Disqus",
                Topic = "Domain Migration finished processing for walterlv.",
                Excerpt = "Setting Hello, walterlv! Your Domain Migration task is completed.",
            },
            new MailGroupViewModel
            {
                Title = "Twitter",
                Topic = "GitHub post a new article",
                Excerpt = "Atlassian Bitbucket .@github, did you Git our cake? Happy 10th anniversary from our team to yours!",
            },
            new MailGroupViewModel
            {
                Title = "Disqus",
                Topic = "Domain Migration finished processing for walterlv.",
                Excerpt = "Setting Hello, walterlv! Your Domain Migration task is completed.",
            },
            new MailGroupViewModel
            {
                Title = "Twitter",
                Topic = "lindexi is reportedly considering asking a new question.",
                Excerpt = "34 events",
            },
            new MailGroupViewModel
            {
                Title = "Disqus",
                Topic = "Domain Migration finished processing for walterlv.",
                Excerpt = "Setting Hello, walterlv! Your Domain Migration task is completed.",
            },
            new MailGroupViewModel
            {
                Title = "Twitter",
                Topic = "GitHub post a new article",
                Excerpt = "Atlassian Bitbucket .@github, did you Git our cake? Happy 10th anniversary from our team to yours!",
            },
            new MailGroupViewModel
            {
                Title = "Disqus",
                Topic = "Domain Migration finished processing for walterlv.",
                Excerpt = "Setting Hello, walterlv! Your Domain Migration task is completed.",
            },
        };

        public static implicit operator MailBoxFolderViewModel(MailBoxFolder source)
        {
            return new MailBoxFolderViewModel
            {
                Name = source.Name,
                FullName = source.FullName,
            };
        }

        public static implicit operator MailBoxFolder(MailBoxFolderViewModel source)
        {
            return new MailBoxFolder
            {
                Name = source.Name,
                FullName = source.FullName,
            };
        }

        [ContractPublicPropertyName(nameof(Name))]
        private string _name;

        [ContractPublicPropertyName(nameof(FullName))]
        private string _fullName;

        [ContractPublicPropertyName(nameof(Separator))]
        private char _separator;
    }
}
