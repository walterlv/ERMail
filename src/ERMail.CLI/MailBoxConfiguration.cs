using System;
using System.Security;
using Walterlv.ERMail.Models;

namespace Walterlv.ERMail
{
    public class MailBoxCliConfiguration
    {
        public MailBoxConnectionInfo Load()
        {
            Console.WriteLine("No user found, create a new one.");

            Console.Write("Email address: ");
            var emailAddress = Console.ReadLine();
            Console.Write("Password: ");
            var password = ReadPassword();
            Console.Write("Incoming(IMAP) and outgoing(SMTP) mail server: ");
            var server = Console.ReadLine();

            return new MailBoxConnectionInfo
            {
                Address = emailAddress,
                UserName = emailAddress,
                Password = password.ToString(),
                IncomingServer = server,
                OutgoingServer = server,
            };
        }

        public SecureString ReadPassword()
        {
            var pwd = new SecureString();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }
    }
}
