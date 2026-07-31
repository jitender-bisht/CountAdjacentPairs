namespace AdjacentPairCounter.Application.DTOs
{
    /// <summary>
    /// The number of non-overlapping adjacent pairs found for a single character.
    /// </summary>
    public class AdjacentPairResponse
    {
        /// <summary>The character that appeared in one or more adjacent pairs.</summary>
        public char Character { get; set; }

        /// <summary>How many non-overlapping adjacent pairs of <see cref="Character"/> were found.</summary>
        public int Count { get; set; }
    }
}
