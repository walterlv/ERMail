using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Walterlv.ERMail.Utils;

namespace Walterlv.ERMail.Classification
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
            var currentWords = CreateWordsDictionary(current);
            _logger.Log($"{string.Join(", ", currentWords.SkipWhile(x => x.Value <= 1).Select(pair => $"{pair.Key}({pair.Value})"))}");

            await all.Skip(1).ForEachAsync(async mail =>
            {
                var words = CreateWordsDictionary(mail);
                var allWords = CreateWordsDictionary(mail, currentWords);
                _logger.Log($"{string.Join(", ", words.SkipWhile(x => x.Value <= 1).Select(pair => $"{pair.Key}({pair.Value})"))}");
                _logger.Log($"{string.Join(", ", allWords.SkipWhile(x => x.Value <= 1).Select(pair => $"{pair.Key}({pair.Value})"))}");
            });
        }

        private Dictionary<string, int> CreateWordsDictionary(string content, Dictionary<string, int> source = null)
        {
            var result = source ?? new Dictionary<string, int>();
            var currentWords = content.Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in currentWords)
            {
                if (!result.TryGetValue(word, out var count))
                {
                    count = 0;
                }
                result[word] = count + 1;
            }

            return result.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
