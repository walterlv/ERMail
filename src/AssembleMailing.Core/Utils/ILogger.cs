using System.Runtime.CompilerServices;

namespace Walterlv.AssembleMailing.Utils
{
    /// <summary>
    /// If a class need a Logger, it should arrange an <see cref="ILogger"/> interface in it's constructor parameter list.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a new message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        void Log(string message, [CallerMemberName] string callerName = null);
    }
}
