using System.Runtime.CompilerServices;

namespace Walterlv.AssembleMailing.Utils
{
    public interface ILogger
    {
        void Log(string message, [CallerMemberName] string callerName = null);
    }
}
