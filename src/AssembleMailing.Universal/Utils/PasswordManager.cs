using Windows.Security.Credentials;

namespace Walterlv.AssembleMailing.Utils
{
    internal class PasswordManager
    {
        private const string MailVaultResourceName = "Walterlv.AssembleMailing";

        internal static string Retrieve(string key)
        {
            var vault = new PasswordVault();
            var credential = vault.Retrieve(MailVaultResourceName, key);
            return credential.Password;
        }

        internal static void Add(string key, string password)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(MailVaultResourceName, key, password));
        }
    }
}
