using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Walterlv.AssembleMailing.Models;

namespace Walterlv.AssembleMailing.Mailing
{
    public class MailBoxConfigurationFile
    {
        public string ConfigFileName { get; }

        public MailBoxConfigurationFile(string configFileName)
        {
            if (configFileName == null)
            {
                throw new ArgumentNullException(nameof(configFileName));
            }

            if (string.IsNullOrWhiteSpace(configFileName))
            {
                throw new ArgumentException("Configuration file name should not be empty.", nameof(configFileName));
            }

            ConfigFileName = configFileName;
        }

        public async Task<MailBoxConfiguration> ReadAsync()
        {
            if (!File.Exists(ConfigFileName)) return new MailBoxConfiguration();

            return await Task.Run(() => Read()).ConfigureAwait(false);
        }

        private MailBoxConfiguration Read()
        {
            var json = JsonSerializer.Create();
            using (var file = File.OpenRead(ConfigFileName))
            using (TextReader reader = new StreamReader(file))
            {
                return json.Deserialize<MailBoxConfiguration>(new JsonTextReader(reader));
            }
        }

        public Task SaveAsync(MailBoxConfiguration configuration)
        {
            return Task.Run(() => Save(configuration));
        }

        private void Save(MailBoxConfiguration configuration)
        {
            var json = JsonSerializer.Create();
            using (var file = File.OpenWrite(ConfigFileName))
            using (TextWriter writer = new StreamWriter(file))
            {
                json.Serialize(writer, configuration);
            }
        }
    }
}
