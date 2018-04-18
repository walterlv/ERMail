namespace Walterlv.AssembleMailing.Models
{
    /// <summary>
    /// Stores a mail box folder info so that we could match the remote folder from the mail server.
    /// This is a model that will be serialized into a file.
    /// </summary>
    public class MailBoxFolder
    {
        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the full folder name. Use this to match the remote folder of the mail server.
        /// </summary>
        public string FullName { get; set; }
    }
}
