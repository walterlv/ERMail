using System;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using Walterlv.AssembleMailing.Models;

namespace Walterlv.AssembleMailing.Mailing
{
    public class IncomingMailClient
    {
        public IncomingMailClient(string userName, string password, string host, int port = 0)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (port < 0)
            {
                throw new ArgumentException(
                    "IncomingServerPort should be larger than 0 (or by default 0).", nameof(port));
            }

            Port = port > 0 ? port : 993;
        }

        public IncomingMailClient(MailBoxConnectionInfo info)
            : this(info.UserName, info.Password, info.IncomingServerHost, info.IncomingServerPort)
        {
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
