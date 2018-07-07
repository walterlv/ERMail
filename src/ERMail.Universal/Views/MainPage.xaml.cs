using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Walterlv.ERMail.Models;
using Walterlv.ERMail.Utils;
using Walterlv.ERMail.ViewModels;

namespace Walterlv.ERMail.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var localFolder = ApplicationData.Current.LocalFolder;
            _configurationFile = new FileSerializor<MailBoxConfiguration>(
                Path.Combine(localFolder.Path, "MailBoxConfiguration.json"));
        }

        private readonly FileSerializor<MailBoxConfiguration> _configurationFile;

        private MainViewModel ViewModel => (MainViewModel) DataContext;
    }
}
