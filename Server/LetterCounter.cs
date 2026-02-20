using System.Collections.Concurrent;

namespace Server
{
    internal class LetterCounter
    {
        private readonly ConcurrentDictionary<char, long> _counts = new();

        internal void UpdateCounter(string message)
        {
            foreach (var c in message)
            {
                if (char.IsLetter(c))
                {
                    var lower = char.ToLowerInvariant(c);
                    _counts.AddOrUpdate(lower, 1, (_, old) => old + 1);
                }
            }
        }

        public override string ToString()
        {
            var ordered = _counts.OrderBy(kv => kv.Key);

            return string.Join(", ", ordered.Select(kv => $"{kv.Key}:{kv.Value}"));
        }
    }
}
