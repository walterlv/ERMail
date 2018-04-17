using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Walterlv.AssembleMailing.Utils
{
    public class FileSerializor<T> where T : new()
    {
        public string FileName { get; }

        public FileSerializor(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Configuration file name should not be empty.", nameof(fileName));
            }

            FileName = fileName;
        }

        public async Task<T> ReadAsync()
        {
            if (!File.Exists(FileName))
            {
                return new T();
            }

            return await Task.Run(() => Read()).ConfigureAwait(false);
        }

        private T Read()
        {
            var json = JsonSerializer.Create();
            using (var file = File.OpenRead(FileName))
            using (TextReader reader = new StreamReader(file))
            {
                return json.Deserialize<T>(new JsonTextReader(reader));
            }
        }

        public Task SaveAsync(T target)
        {
            return Task.Run(() => Save(target));
        }

        internal void Save(T target)
        {
            var json = JsonSerializer.Create();
            using (var file = File.OpenWrite(FileName))
            using (TextWriter writer = new StreamWriter(file))
            {
                json.Serialize(writer, target);
            }
        }
    }
}
