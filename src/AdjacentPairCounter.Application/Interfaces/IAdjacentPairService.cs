using AdjacentPairCounter.Application.DTOs;

namespace AdjacentPairCounter.Application.Interfaces
{
    /// <summary>
    /// Counts adjacent (consecutive, non-overlapping) duplicate characters within a string.
    /// </summary>
    public interface IAdjacentPairService
    {
        /// <summary>
        /// Scans <paramref name="input"/> left to right and counts, per character, how many
        /// non-overlapping adjacent pairs occur (e.g. "AAAA" -> two pairs of 'A').
        /// </summary>
        /// <param name="input">The string to scan. Null/empty/whitespace yields no results.</param>
        /// <returns>One <see cref="AdjacentPairResponse"/> per character that has at least one adjacent pair.</returns>
        IEnumerable<AdjacentPairResponse> Count(string input);
    }
}
