using System.Collections.Generic;

namespace Walterlv.AssembleMailing.Models
{
    public class MailBoxConfiguration
    {
        public IList<MailBoxConnectionInfo> Connections { get; set; } = new List<MailBoxConnectionInfo>();
    }
}
