using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Walterlv.ERMail.Utils
{
    /// <summary>
    /// Serialize or deserialize a class instance using json format.
    /// </summary>
    /// <typeparam name="T">The type which is preparing to serialize. It can be am IList.</typeparam>
    public class FileSerializor<T> where T : new()
    {
        /// <summary>
        /// Gets the serialization file name (full path).
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Initialize a new instance of <see cref="FileSerializor{T}"/>.
        /// </summary>
        /// <param name="fileName"></param>
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

        /// <summary>
        /// Read an instance from file. If the file does not exist, it will return a new one (with all fields default).
        /// </summary>
        /// <returns></returns>
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
            using (var file = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Write))
            using (TextReader reader = new StreamReader(file))
            {
                return json.Deserialize<T>(new JsonTextReader(reader));
            }
        }

        /// <summary>
        /// Save the target object into a file. If the file does not exists, a new one will be created.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Task SaveAsync(T target)
        {
            return Task.Run(() => Save(target));
        }

        private void Save(T target)
        {
            var json = JsonSerializer.Create();
            var directory = new FileInfo(FileName).Directory;
            if (directory?.Exists is false)
            {
                directory.Create();
            }
            using (var file = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (TextWriter writer = new StreamWriter(file))
            {
                json.Serialize(writer, target);
            }
        }
    }
}
