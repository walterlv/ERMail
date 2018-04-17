using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using Walterlv.AssembleMailing.Models;

namespace Walterlv.AssembleMailing.Mailing
{
    public class MailBoxCache
    {
        private static readonly ConcurrentDictionary<string, MailBoxCache> AllCache
            = new ConcurrentDictionary<string, MailBoxCache>();

        public static MailBoxCache Get(string directory, MailBoxConnectionInfo info, IPasswordManager passwordManager)
        {
            if (!AllCache.TryGetValue(info.Address, out var cache))
            {
                cache = new MailBoxCache(Path.Combine(directory, info.Address), info, passwordManager);
                AllCache[info.Address] = cache;
            }

            return cache;
        }

        public MailBoxCache(string directory, MailBoxConnectionInfo info, IPasswordManager passwordManager)
        {
            Directory = directory;
            ConnectionInfo = info;
            _passwordManager = passwordManager;
        }

        public string Directory { get; }

        public MailBoxConnectionInfo ConnectionInfo { get; }

        public async Task<IList<MailBoxFolder>> LoadMailFoldersAsync()
        {
            FillPassword(ConnectionInfo);
            var result = new List<MailBoxFolder>();
            using (var client = await new IncomingMailClient(ConnectionInfo).ConnectAsync())
            {
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0]);
                foreach (var folder in new[] {inbox}.Union(folders))
                {
                    result.Add(new MailBoxFolder
                    {
                        Name = folder.Name,
                        FullName = folder.FullName,
                    });
                }
            }

            return result;
        }

        public async Task LoadMailAsync()
        {

        }

        private void FillPassword(MailBoxConnectionInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Address))
            {
                info.Password = _passwordManager.Retrieve(info.Address);
            }
        }

        private readonly IPasswordManager _passwordManager;
    }
}
