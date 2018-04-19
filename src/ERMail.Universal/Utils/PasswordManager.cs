using Windows.Security.Credentials;
using Walterlv.ERMail.Mailing;

namespace Walterlv.ERMail.Utils
{
    internal class PasswordManager : IPasswordManager
    {
        private const string MailVaultResourceName = "Walterlv.ERMail";

        internal static IPasswordManager Current = new PasswordManager();

        string IPasswordManager.Retrieve(string key)
        {
            var vault = new PasswordVault();
            var credential = vault.Retrieve(MailVaultResourceName, key);
            return credential.Password;
        }

        void IPasswordManager.Add(string key, string password)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(MailVaultResourceName, key, password));
        }
    }
}
