using System;
using System.Security;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;

namespace Walterlv.AssembleMailing.Mailing
{
    public class MailClient
    {
        public MailClient(string host, int port, string userName, string password)
        {
            Host = host;
            Port = port;
            UserName = userName;
            Password = new SecureString();
            foreach (var @char in password)
            {
                Password.AppendChar(@char);
            }
        }

        public string Host { get; }

        public int Port { get; }

        public string UserName { get; }

        public SecureString Password { get; }

        public async Task ConnectAsync()
        {
            using (var client = new ImapClient())
            {
                // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(Host, Port, true);

                client.Authenticate(UserName, Password.ToString());

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }

                client.Disconnect(true);
            }
        }
    }
}
