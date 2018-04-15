using System.Threading.Tasks;
using MailKit.Net.Imap;

namespace Walterlv.AssembleMailing.Mailing
{
    public class MailClient
    {
        public MailClient(string userName, string password, string host, int? port = null)
        {
            UserName = userName;
            Password = password;
            Host = host;
            Port = port ?? 993;
        }

        public string Host { get; }

        public int Port { get; }

        public string UserName { get; }

        public string Password { get; }

        public async Task TestConnectionAsync()
        {
            using (var client = new ImapClient())
            {
                // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(Host, Port, true);
                await client.AuthenticateAsync(UserName, Password);
                client.Disconnect(true);
            }
        }

        public async Task<ImapClient> ConnectAsync()
        {
            var client = new ImapClient();
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(Host, Port, true);
            await client.AuthenticateAsync(UserName, Password);
            return client;
        }
    }
}
