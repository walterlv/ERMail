using System;
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
        static async Task Main(string[] args)
        {
            // Starting.
            Console.WriteLine("ERMail | version 0.1.0");
            Console.WriteLine("Copyright (C) 2018 Walterlv. All rightes reserved.");
            Console.WriteLine("--------------------------------------------------");

            // Prepare the configuration folder.
            var localFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ermail");

            // Load or input connection info.
            var configurationFile = new FileSerializor<MailBoxConfiguration>(
                Path.Combine(localFolder, "MailBoxConfiguration.json"));
            var mailBoxConfiguration = await configurationFile.ReadAsync();
            if (!mailBoxConfiguration.Connections.Any())
            {
                var mailBoxCliConfiguration = new MailBoxCliConfiguration();
                var info = mailBoxCliConfiguration.Load();
                mailBoxConfiguration.Connections.Add(info);
                await configurationFile.SaveAsync(mailBoxConfiguration);
            }

            var connectionInfo = mailBoxConfiguration.Connections.First();
            var cache = MailBoxCache.Get(localFolder, connectionInfo, PasswordManager.Current);

            // Load and select folders.
            var folders = await cache.LoadMailFoldersAsync();
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
            
            // Download mails.
        }
    }
}
