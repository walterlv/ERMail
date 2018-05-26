using System;
using System.Collections.Async;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Walterlv.ERMail.Mailing;
using Walterlv.ERMail.Models;
using Walterlv.ERMail.Utils;

namespace Walterlv.ERMail
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Starting.
            Console.WriteLine("ERMail | version 0.1.0");
            Console.WriteLine("Copyright (C) 2018 Walterlv. All rightes reserved.");
            Console.WriteLine("--------------------------------------------------");

            try
            {
                await RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                return -1;
            }
        }

        private static async Task RunAsync()
        {
            // Prepare the configuration folder.
            var localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ermail");

            // Load or input connection info.
            var connectionInfo = await ReadConnectionInfo(localFolder);

            var cache = MailBoxCache.Get(localFolder, connectionInfo, PasswordManager.Current);

            // Load and select folders.
            var currentFolderConfigurationFile =
                Path.Combine(localFolder, connectionInfo.Address, "CurrentFolder.json");
            var folder = await SelectFolder(currentFolderConfigurationFile, cache);

            // Download mails.
            var mails = cache.EnumerateMailDetailsAsync(folder);
            Console.WriteLine("Start downloading mails.");
            await HandleMails(mails, Path.Combine(localFolder, "Attachments", "{topic}{ext}"));
            Console.WriteLine("All mails are downloaded.");
        }

        private static async Task HandleMails(IAsyncEnumerable<MailContentCache> mails, string fileFormat)
        {
            await mails.ForEachAsync(async mail =>
            {
                if (mail.AttachmentFileNames.Any())
                {
                    foreach (var attachment in mail.AttachmentFileNames)
                    {
                        var fileName = fileFormat.Replace("{topic}", mail.Topic, StringComparison.OrdinalIgnoreCase);
                        fileName = fileName.Replace("{ext}", Path.GetExtension(attachment), StringComparison.OrdinalIgnoreCase);
                        var directory = Path.GetDirectoryName(fileName);
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        File.Copy(attachment, fileName, true);
                    }
                    Console.WriteLine($"Downloaded: [{mail.Topic}]");
                    Console.Write("Downloading...");
                    Console.CursorLeft = 0;
                }
                else
                {
                    Console.Write($"No attachment found: [{string.Join("", mail.Topic.Take(20))}]");
                    Console.CursorLeft = 0;
                }
            });
        }

        private static async Task<MailBoxConnectionInfo> ReadConnectionInfo(string localFolder)
        {
            var configurationFile = new FileSerializor<MailBoxConfiguration>(
                Path.Combine(localFolder, "MailBoxConfiguration.json"));
            var mailBoxConfiguration = await configurationFile.ReadAsync();
            if (!mailBoxConfiguration.Connections.Any())
            {
                var mailBoxCliConfiguration = new MailBoxCliConfiguration();
                var info = await mailBoxCliConfiguration.Load();
                mailBoxConfiguration.Connections.Add(info);
                PasswordManager.Current.Add(info.Address, info.Password);
                await configurationFile.SaveAsync(mailBoxConfiguration);
            }

            var connectionInfo = mailBoxConfiguration.Connections.First();
            return connectionInfo;
        }

        private static async Task<MailBoxFolder> SelectFolder(string configurationFile, MailBoxCache cache)
        {
            var currentFolderFile = new FileSerializor<FolderInfo>(configurationFile);
            var currentFolderInfo = await currentFolderFile.ReadAsync();

            var folders = await cache.LoadMailFoldersAsync();

            var storedFolder = folders.FirstOrDefault(x=>x.FullName == currentFolderInfo.CurrentFolder);
            if (storedFolder != null)
            {
                Console.WriteLine($"Your current folder is {storedFolder.FullName}.");
                return storedFolder;
            }

            Console.WriteLine("Find these folders in your mailbox:");
            for (var i = 0; i < folders.Count; i++)
            {
                var folder = folders[i];
                Console.WriteLine($"[{(i + 1).ToString().PadLeft(2, ' ')}] {folder.FullName}");
            }

            var selection = 0;
            while (selection <= 0 || selection > folders.Count)
            {
                Console.Write($"Please select the folder you want in [1~{folders.Count}]: ");
                int.TryParse(Console.ReadLine(), out selection);
            }

            var mailBoxFolder = folders[selection - 1];
            currentFolderInfo.CurrentFolder = mailBoxFolder.FullName;
            await currentFolderFile.SaveAsync(currentFolderInfo);
            return mailBoxFolder;
        }
    }

    public class FolderInfo
    {
        public string CurrentFolder { get; set; }
    }
}
