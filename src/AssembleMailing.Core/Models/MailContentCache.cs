namespace Walterlv.AssembleMailing.Models
{
    public class MailContentCache
    {
        public string HtmlBody { get; }

        public MailContentCache(string htmlBody)
        {
            HtmlBody = htmlBody;
        }
    }
}
