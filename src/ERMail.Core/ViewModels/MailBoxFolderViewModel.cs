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

        public ObservableCollection<MailGroupViewModel> Mails { get; } = new ObservableCollection<MailGroupViewModel>();

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MailBoxFolderViewModel) obj);
        }

        protected bool Equals(MailBoxFolderViewModel other)
        {
            return string.Equals(_fullName, other._fullName);
        }

        public override int GetHashCode()
        {
            return (_fullName != null ? _fullName.GetHashCode() : 0);
        }

        public static bool operator ==(MailBoxFolderViewModel left, MailBoxFolderViewModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MailBoxFolderViewModel left, MailBoxFolderViewModel right)
        {
            return !Equals(left, right);
        }
    }
}
