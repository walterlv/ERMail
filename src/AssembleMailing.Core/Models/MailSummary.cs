using System.Collections.Generic;

namespace Walterlv.AssembleMailing.Models
{
    public class MailSummary
    {
        public string Title { get; set; }
        public string Topic { get; set; }
        public string Excerpt { get; set; }
        public IList<uint> MailIds { get; set; }
    }
}
