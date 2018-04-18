using System.Collections.Generic;

namespace Walterlv.AssembleMailing.Models
{
    /// <summary>
    /// Stores connection information of all mail boxes.
    /// This is a model that will be serialized into a file.
    /// </summary>
    public class MailBoxConfiguration
    {
        /// <summary>
        /// Gets or sets all connection information of all mail boxes.
        /// </summary>
        public IList<MailBoxConnectionInfo> Connections { get; set; } = new List<MailBoxConnectionInfo>();
    }
}
