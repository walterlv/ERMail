using System.Collections.Generic;
using Walterlv.ERMail.Utils;

namespace Walterlv.ERMail
{
    internal class PasswordManager : IPasswordManager
    {
        internal static readonly IPasswordManager Current = new PasswordManager();

        private readonly Dictionary<string, string> _passwords = new Dictionary<string, string>();

        public string Retrieve(string key)
        {
            return "****************";
        }

        public void Add(string key, string password)
        {
            _passwords[key] = password;
        }
    }
}
