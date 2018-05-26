using System.Collections.Generic;

namespace Walterlv.ERMail.Models
{
    /// <summary>
    /// Stores the fetched mail content of a mail.
    /// This is a model that will be serialized into a file.
    /// </summary>
    public class MailContentCache
    {
        /// <summary>
        /// Gets or sets the topic of the mail.
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Gets or sets the mail plain text content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the mail html file name.
        /// It is recommended to store the file name in relative path.
        /// </summary>
        public string HtmlFileName { get; set; }

        /// <summary>
        /// Gets or sets the attachment file names.
        /// </summary>
        public List<string> AttachmentFileNames { get; set; } = new List<string>();
    }
}
