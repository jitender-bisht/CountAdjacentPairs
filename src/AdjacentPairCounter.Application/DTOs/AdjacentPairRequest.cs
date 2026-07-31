namespace AdjacentPairCounter.Application.DTOs
{
    /// <summary>
    /// Request payload for the adjacent-pair count endpoint.
    /// </summary>
    public class AdjacentPairRequest
    {
        /// <summary>The string to scan for adjacent duplicate characters.</summary>
        public string Input { get; set; } = string.Empty;
    }
}
