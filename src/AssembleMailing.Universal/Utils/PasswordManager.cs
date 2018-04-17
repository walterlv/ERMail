using Windows.Security.Credentials;
using Walterlv.AssembleMailing.Mailing;

namespace Walterlv.AssembleMailing.Utils
{
    internal class PasswordManager : IPasswordManager
    {
        private const string MailVaultResourceName = "Walterlv.AssembleMailing";

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
