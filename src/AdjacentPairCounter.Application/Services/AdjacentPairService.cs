using AdjacentPairCounter.Application.DTOs;
using AdjacentPairCounter.Application.Interfaces;

namespace AdjacentPairCounter.Application.Services
{
    /// <inheritdoc cref="IAdjacentPairService"/>
    public class AdjacentPairService : IAdjacentPairService
    {
        /// <inheritdoc/>
        public IEnumerable<AdjacentPairResponse> Count(string input)
        {
            var result = new Dictionary<char, int>();

            if (string.IsNullOrWhiteSpace(input))
            {
                return Enumerable.Empty<AdjacentPairResponse>();
            }

            int index = 0;

            // Single left-to-right pass: when a pair is matched, jump past both
            // characters so e.g. "AAAA" counts as two pairs rather than three
            // overlapping ones ("AA" at 0-1 and 1-2 would double count 'A' at index 1).
            while (index < input.Length - 1)
            {
                if (input[index] == input[index + 1])
                {
                    char character = input[index];

                    if (result.ContainsKey(character))
                    {
                        result[character]++;
                    }
                    else
                    {
                        result.Add(character, 1);
                    }

                    // Skip the next character to avoid overlapping pairs
                    index += 2;
                }
                else
                {
                    index++;
                }
            }

            return result.Select(x => new AdjacentPairResponse
            {
                Character = x.Key,
                Count = x.Value
            });
        }
    }
}