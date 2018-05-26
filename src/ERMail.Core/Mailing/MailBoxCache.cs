using System;
using System.Collections.Async;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit;
using MimeKit;
using Walterlv.ERMail.Models;
using Walterlv.ERMail.Utils;

namespace Walterlv.ERMail.Mailing
{
    /// <summary>
    /// Manage the cache of all mail data.
    /// It also fetch data from the mail server, so that it can determine when to update the cache.
    /// </summary>
    public class MailBoxCache
    {
        /// <summary>
        /// Stores all cache manager indexed by mail address.
        /// </summary>
        private static readonly ConcurrentDictionary<string, MailBoxCache> AllCache
            = new ConcurrentDictionary<string, MailBoxCache>();

        /// <summary>
        /// Gets a <see cref="MailBoxCache"/> instance that is auto managed.
        /// Returns value will never be null.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="info"></param>
        /// <param name="passwordManager"></param>
        /// <returns></returns>
        public static MailBoxCache Get(string directory, MailBoxConnectionInfo info, IPasswordManager passwordManager)
        {
            if (!AllCache.TryGetValue(info.Address, out var cache))
            {
                cache = new MailBoxCache(Path.Combine(directory, info.Address), info, passwordManager);
                AllCache[info.Address] = cache;
            }

            return cache;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="MailBoxCache"/>.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="info"></param>
        /// <param name="passwordManager"></param>
        public MailBoxCache(string directory, MailBoxConnectionInfo info, IPasswordManager passwordManager)
        {
            Directory = directory;
            _connectionInfo = info;
            _passwordManager = passwordManager;
        }

        /// <summary>
        /// Gets the directory where this cache manager should store files in.
        /// If it is auto managed, the directory is named like user@example.com.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Load all folders of current mail cache.
        /// If any cache exists, it will return the cache and then **TODO** fetch them from the mail server later.
        /// If the cache does not exists, it will **TODO** return an empty result and then fetch all from the mail server.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<MailBoxFolder>> LoadMailFoldersAsync()
        {
            var folderCache = new FileSerializor<List<MailBoxFolder>>(Path.Combine(Directory, "folders.json"));
            var cachedFolder = await folderCache.ReadAsync();
            if (cachedFolder.Any())
            {
                return cachedFolder;
            }

            FillPassword(_connectionInfo);
            var result = new List<MailBoxFolder>();
            using (var client = await new IncomingMailClient(_connectionInfo).ConnectAsync())
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

            await folderCache.SaveAsync(result);

            return result;
        }

        /// <summary>
        /// Load a range of mail summaries from a specified mail folder cache.
        /// If any cache exists, it will return the cache and then **TODO** fetch them from the mail server later.
        /// If the cache does not exists, it will **TODO** return an empty result and then fetch all from the mail server.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task<IList<MailSummary>> LoadMailsAsync(MailBoxFolder folder, int start = 0, int length = 100)
        {
            var cache = new FileSerializor<List<MailSummary>>(
                Path.Combine(Directory, "Folders", folder.FullName, "summaries.json"));
            if (start == 0)
            {
                // Temporarily load cache only for first 100.
                var cachedSummary = await cache.ReadAsync();
                if (cachedSummary.Any())
                {
                    return cachedSummary;
                }
            }

            FillPassword(_connectionInfo);
            var result = new List<MailSummary>();
            using (var client = await new IncomingMailClient(_connectionInfo).ConnectAsync())
            {
                var mailFolder = await client.GetFolderAsync(folder.FullName);
                mailFolder.Open(FolderAccess.ReadOnly);

                var fetchingCount = mailFolder.Count < start + length ? mailFolder.Count : start + length;
                if (fetchingCount > 0)
                {
                    var messageSummaries = await mailFolder.FetchAsync(mailFolder.Count - fetchingCount,
                        mailFolder.Count - 1 - start,
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
                            MailIds = new List<uint> {summary.UniqueId.Id},
                        };
                        result.Add(mailGroup);
                    }
                }
            }

            await cache.SaveAsync(result);

            return result;
        }

        /// <summary>
        /// Load a mail content from a specified mail folder cache.
        /// If any cache exists, it will return the cache.
        /// But if it does not exists, it will fetch one from the mail server (and then cache it).
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MailContentCache> LoadMailAsync(MailBoxFolder folder, uint id)
        {
            var htmlFileName = Path.Combine(Directory, "Mails", $"{id}.html");
            var contentfileName = Path.Combine(Directory, "Mails", $"{id}.json");
            var cache = new FileSerializor<MailContentCache>(contentfileName);

            var contentCache = await cache.ReadAsync();
            if (contentCache.Content != null && File.Exists(contentCache.HtmlFileName))
            {
                return contentCache;
            }


            FillPassword(_connectionInfo);
            using (var client = await new IncomingMailClient(_connectionInfo).ConnectAsync())
            {
                var mailFolder = await client.GetFolderAsync(folder.FullName);
                mailFolder.Open(FolderAccess.ReadOnly);

                var message = await mailFolder.GetMessageAsync(new UniqueId(id));
                var htmlBody = message.HtmlBody;

                var content = new MailContentCache
                {
                    Topic = message.Subject,
                    Content = message.TextBody ?? htmlBody,
                    HtmlFileName = htmlFileName,
                };
                await cache.SaveAsync(content);
                File.WriteAllText(htmlFileName, htmlBody);

                return content;
            }
        }

        /// <summary>
        /// Enumerate all mail contents asynchronously from the specified folder via the cache strategy.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public IAsyncEnumerable<MailContentCache> EnumerateMailsAsync(MailBoxFolder folder)
        {
            return new AsyncEnumerable<MailContentCache>(async yield =>
            {
                try
                {
                    var startIndex = 0;
                    while (true)
                    {
                        var summaries = await LoadMailsAsync(folder, startIndex, startIndex + 20);
                        var finished = true;
                        foreach (var summary in summaries)
                        {
                            finished = false;
                            var contentCache = await LoadMailAsync(folder, summary.MailIds.First());
                            await yield.ReturnAsync(contentCache);
                        }

                        startIndex += 20;

                        if (finished)
                        {
                            break;
                        }
                    }
                    yield.Break();
                }
                catch (Exception ex)
                {
                    yield.Break();
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            });
        }


        public async Task<MailContentCache> DownloadMailAsync(MailBoxFolder folder, uint id, Action<long, long> reportProgress)
        {
            var contentfileName = Path.Combine(Directory, "Mails", $"{id}.json");
            var htmlFileName = Path.Combine(Directory, "Mails", $"{id}.html");
            var attachmentDirectory = Path.Combine(Directory, "Mails", $"{id}.attachments");
            var cache = new FileSerializor<MailContentCache>(contentfileName);
            var contentCache = await cache.ReadAsync();
            if (contentCache.Content != null && File.Exists(contentCache.HtmlFileName))
            {
                return contentCache;
            }

            FillPassword(_connectionInfo);
            using (var client = await new IncomingMailClient(_connectionInfo).ConnectAsync())
            {
                var mailFolder = await client.GetFolderAsync(folder.FullName);
                mailFolder.Open(FolderAccess.ReadOnly);

                var message = await mailFolder.GetMessageAsync(new UniqueId(id));
                var htmlBody = message.HtmlBody;
                var attachments = message.Attachments.Select(x => (x.ContentDisposition.FileName, x));
                var attachmentFiles = new List<string>();
                foreach (var (fileName, attachment) in attachments)
                {
                    if (!System.IO.Directory.Exists(attachmentDirectory))
                    {
                        System.IO.Directory.CreateDirectory(attachmentDirectory);
                    }
                    var tempFileName = $"{fileName}.downloading";
                    var tempFile = Path.Combine(attachmentDirectory, tempFileName);
                    var file = Path.Combine(attachmentDirectory, fileName);
                    await attachment.WriteToAsync(tempFile);

                    File.Move(tempFile, file);
                    attachmentFiles.Add(file);
                }

                var content = new MailContentCache
                {
                    Topic = message.Subject,
                    Content = message.TextBody ?? htmlBody,
                    HtmlFileName = htmlFileName,
                    AttachmentFileNames = attachmentFiles,
                };
                await cache.SaveAsync(content);
                File.WriteAllText(htmlFileName, htmlBody);

                return content;
            }
        }

        public IAsyncEnumerable<MailContentCache> EnumerateMailDetailsAsync(MailBoxFolder folder, Action<long, long> reportProgress)
        {
            return new AsyncEnumerable<MailContentCache>(async yield =>
            {
                try
                {
                    var startIndex = 0;
                    while (true)
                    {
                        var summaries = await LoadMailsAsync(folder, startIndex, startIndex + 20);
                        var finished = true;
                        foreach (var summary in summaries)
                        {
                            finished = false;
                            var contentCache = await DownloadMailAsync(folder, summary.MailIds.First(), reportProgress);
                            await yield.ReturnAsync(contentCache);
                        }

                        startIndex += 20;

                        if (finished)
                        {
                            break;
                        }
                    }
                    yield.Break();
                }
                catch (Exception ex)
                {
                    yield.Break();
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            });
        }

        private void FillPassword(MailBoxConnectionInfo info)
        {
            if (!string.IsNullOrWhiteSpace(info.Address))
            {
                info.Password = _passwordManager.Retrieve(info.Address);
            }
        }

        private readonly IPasswordManager _passwordManager;
        private readonly MailBoxConnectionInfo _connectionInfo;
    }
}
