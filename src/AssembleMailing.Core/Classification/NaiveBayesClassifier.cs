using System.Collections.Async;
using System.Threading.Tasks;
using Walterlv.AssembleMailing.Utils;

namespace Walterlv.AssembleMailing.Classification
{
    public class NaiveBayesClassifier
    {
        private readonly ILogger _logger;

        public NaiveBayesClassifier(ILogger logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(string current, IAsyncEnumerable<string> all)
        {
            await all.Skip(1).ForEachAsync(async mail =>
            {
                if (mail.Length > 10)
                {
                    mail = mail.Substring(0, 10);
                }
                _logger.Log($"Mail fetched. Id={mail}");
            });
        }
    }
}
