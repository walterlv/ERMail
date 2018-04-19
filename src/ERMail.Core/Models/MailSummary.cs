using System.Collections.Generic;

namespace Walterlv.ERMail.Models
{
    /// <summary>
    /// Stores the mail summary.
    /// </summary>
    public class MailSummary
    {
        /// <summary>
        /// Gets or sets the title of a mail.
        /// In this app, it is the name of the Envelope.From.
        /// TODO **It's improper!**
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the topic of a mail.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the excerpt of a mail.
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Gets or sets the mail ids of this mail group summary.
        /// TODO It's improper because we should stores the origin data so that we could change our Views in an easier way.
        /// </summary>
        public IList<uint> MailIds { get; set; }
    }
}
