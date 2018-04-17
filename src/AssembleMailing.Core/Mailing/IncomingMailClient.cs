using System;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using Walterlv.AssembleMailing.Models;

namespace Walterlv.AssembleMailing.Mailing
{
    /// <summary>
    /// A mail client that can receive mails.
    /// </summary>
    public class IncomingMailClient
    {
        /// <summary>
        /// Initialize a new instance of <see cref="IncomingMailClient"/>.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public IncomingMailClient(string userName, string password, string host, int port = 0)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (port < 0)
            {
                throw new ArgumentException(
                    "IncomingServerPort should be larger than 0 (or by default 0).", nameof(port));
            }

            Port = port > 0 ? port : 993;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="IncomingMailClient"/> using the specified connection info.
        /// </summary>
        /// <param name="info"></param>
        public IncomingMailClient(MailBoxConnectionInfo info)
            : this(info.UserName, info.Password, info.IncomingServerHost, info.IncomingServerPort)
        {
        }

        /// <summary>
        /// Get the incoming host. Like `example.com`.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Get the incoming port.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Get the mail user name.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Get the user password.
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Test connection. Just connect and then disconnect the mail server.
        /// If test failed, an exception will throw.
        /// </summary>
        /// <returns></returns>
        public async Task TestConnectionAsync()
        {
            using (var client = new ImapClient())
            {
                // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(Host, Port, true);
                await client.AuthenticateAsync(UserName, _password);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Connect the mail server to receive mails.
        /// Please call this method using the `using` grammar.
        /// <para/>
        /// <code>
        /// using(var client = await mailClient.ConnectAsync())
        /// </code>
        /// </summary>
        /// <returns></returns>
        public async Task<ImapClient> ConnectAsync()
        {
            var client = new ImapClient();
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(Host, Port, true);
            await client.AuthenticateAsync(UserName, _password);
            return client;
        }
    }
}
