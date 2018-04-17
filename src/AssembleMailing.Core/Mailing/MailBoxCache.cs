using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using Walterlv.AssembleMailing.Models;
using Walterlv.AssembleMailing.ViewModels;

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

        public async Task<IList<MailSummary>> LoadMailsAsync(MailBoxFolder folder)
        {
            FillPassword(ConnectionInfo);
            var result = new List<MailSummary>();
            using (var client = await new IncomingMailClient(ConnectionInfo).ConnectAsync())
            {
                var mailFolder = await client.GetFolderAsync(folder.FullName);
                mailFolder.Open(FolderAccess.ReadOnly);

                var messageSummaries = await mailFolder.FetchAsync(mailFolder.Count - 20, mailFolder.Count - 1,
                    MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
                foreach (var summary in messageSummaries.Reverse())
                {
                    TextPart body;
                    try
                    {
                        body = (TextPart) await mailFolder.GetBodyPartAsync(summary.UniqueId, summary.TextBody);
                    }
                    catch (Exception ex)
                    {
                        // Temporarily catch all exceptions, and it will be handled correctly after main project is about to finish.
                        body = null;
                    }

                    var mailGroup = new MailSummary
                    {
                        Title = summary.Envelope.From.Select(x => x.Name).FirstOrDefault() ?? "(Anonymous)",
                        Topic = summary.Envelope.Subject,
                        Excerpt = body?.Text?.Replace(Environment.NewLine, " "),
                    };
                    mailGroup.MailIds.Add(summary.UniqueId.Id);
                    result.Add(mailGroup);
                }
            }

            return result;
        }

        public async Task<MailContentCache> LoadMailAsync(uint id)
        {
            FillPassword(ConnectionInfo);
            using (var client = await new IncomingMailClient(ConnectionInfo).ConnectAsync())
            {
                client.Inbox.Open(FolderAccess.ReadOnly);
                try
                {
                    var message = await client.Inbox.GetMessageAsync(new UniqueId(id));
                    var htmlBody = message.HtmlBody;
                    return new MailContentCache(htmlBody);
                }
                catch (Exception ex)
                {
                    // Temporarily catch all exceptions, and it will be handled correctly after main project is about to finish.
                    return null;
                }
            }
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
