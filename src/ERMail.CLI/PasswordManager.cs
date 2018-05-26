using System;
using System.IO;
using System.Security.Cryptography;
using Walterlv.ERMail.Utils;

namespace Walterlv.ERMail
{
    internal class PasswordManager : IPasswordManager
    {
        internal static readonly IPasswordManager Current = new PasswordManager();

        private static readonly string LocalFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ermail");

        public string Retrieve(string key)
        {
            var keyFile = Path.Combine(LocalFolder, "key");
            var keyBytes = ReadOrGenerateRandomKeyFile(keyFile);

            var passwordFile = Path.Combine(LocalFolder, key, "token");
            var encrypted = File.ReadAllBytes(passwordFile);

            var password = DecryptStringFromBytes(encrypted, keyBytes);
            return password;
        }

        public void Add(string key, string password)
        {
            var keyFile = Path.Combine(LocalFolder, "key");
            var keyBytes = ReadOrGenerateRandomKeyFile(keyFile);
            var encrypted = EncryptStringToBytes(password, keyBytes);
            var passwordFile = Path.Combine(LocalFolder, key, "token");
            if (!Directory.Exists(Path.GetDirectoryName(passwordFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(passwordFile));
            }
            File.WriteAllBytes(passwordFile, encrypted);
        }

        private byte[] ReadOrGenerateRandomKeyFile(string keyFile)
        {
            if (!File.Exists(keyFile))
            {
                using (var random = new RNGCryptoServiceProvider())
                {
                    var keyBytes = new byte[16];
                    random.GetBytes(keyBytes);
                    File.WriteAllBytes(keyFile, keyBytes);
                    return keyBytes;
                }
            }

            return File.ReadAllBytes(keyFile);
        }

        static byte[] EncryptStringToBytes(string plainText, byte[] Key)
        {
            byte[] encrypted;
            byte[] iv;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                aesAlg.GenerateIV();
                iv = aesAlg.IV;

                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            var combinedIvCt = new byte[iv.Length + encrypted.Length];
            Array.Copy(iv, 0, combinedIvCt, 0, iv.Length);
            Array.Copy(encrypted, 0, combinedIvCt, iv.Length, encrypted.Length);

            // Return the encrypted bytes from the memory stream. 
            return combinedIvCt;

        }

        static string DecryptStringFromBytes(byte[] cipherTextCombined, byte[] key)
        {
            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an Aes object 
            // with the specified key and IV. 
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                var iv = new byte[aesAlg.BlockSize / 8];
                var cipherText = new byte[cipherTextCombined.Length - iv.Length];

                Array.Copy(cipherTextCombined, iv, iv.Length);
                Array.Copy(cipherTextCombined, iv.Length, cipherText, 0, cipherText.Length);

                aesAlg.IV = iv;

                aesAlg.Mode = CipherMode.CBC;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }
    }
}
