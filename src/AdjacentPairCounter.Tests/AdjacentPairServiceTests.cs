using AdjacentPairCounter.Application.Services;

namespace AdjacentPairCounter.Tests
{
    /// <summary>
    /// Unit tests for <see cref="AdjacentPairService"/>, covering edge cases (empty/whitespace/single-char
    /// input), overlap handling, and mixed/numeric/special/case-sensitive character sets.
    /// </summary>
    public class AdjacentPairServiceTests
    {
        private readonly AdjacentPairService _service = new();

        [Fact]
        public void Count_ShouldReturnEmpty_WhenInputIsWhiteSpace()
        {
            var result = _service.Count("   ");

            Assert.Empty(result);
        }

        [Fact]
        public void Count_ShouldReturnEmpty_WhenInputIsEmpty()
        {
            var result = _service.Count(string.Empty);

            Assert.Empty(result);
        }

        [Fact]
        public void Count_ShouldReturnEmpty_WhenInputHasSingleCharacter()
        {
            var result = _service.Count("A");
            Assert.Empty(result);
        }

        [Fact]
        public void Count_ShouldDetectPairAtEndOfString()
        {
            var result = _service.Count("ABCC").ToList();

            Assert.Single(result);
            Assert.Equal('C', result[0].Character);
            Assert.Equal(1, result[0].Count);
        }

        [Fact]
        public void Count_ShouldIgnoreOverlappingPairs()
        {
            var result = _service.Count("AAA").ToList();

            Assert.Single(result);
            Assert.Equal('A', result[0].Character);
            Assert.Equal(1, result[0].Count);
        }

        [Fact]
        public void Count_ShouldHandleLongInput()
        {
            var input = new string('A', 10000);
            var result = _service.Count(input).ToList();

            Assert.Single(result);
            Assert.Equal(5000, result[0].Count);
        }

        [Fact]
        public void Count_ShouldReturnEmpty_WhenNoAdjacentPairsExist()
        {
            var result = _service.Count("ABCDEF");
            Assert.Empty(result);
        }

        [Fact]
        public void Count_ShouldReturnOnePair()
        {
            var result = _service.Count("AAB").ToList();

            Assert.Single(result);
            Assert.Equal('A', result[0].Character);
            Assert.Equal(1, result[0].Count);
        }

        [Fact]
        public void Count_ShouldReturnMultiplePairs()
        {
            var result = _service.Count("AABBCC").ToList();

            Assert.Equal(3, result.Count);

            Assert.Contains(result, x => x.Character == 'A' && x.Count == 1);
            Assert.Contains(result, x => x.Character == 'B' && x.Count == 1);
            Assert.Contains(result, x => x.Character == 'C' && x.Count == 1);
        }

        [Fact]
        public void Count_ShouldCountMultiplePairsOfSameCharacter()
        {
            var result = _service.Count("AAAABB").ToList();

            Assert.Equal(2, result.Count);

            Assert.Contains(result, x => x.Character == 'A' && x.Count == 2);
            Assert.Contains(result, x => x.Character == 'B' && x.Count == 1);
        }

        [Fact]
        public void Count_ShouldHandleMixedCharacters()
        {
            var result = _service.Count("AABBBasas").ToList();

            Assert.Equal(2, result.Count);

            Assert.Contains(result, x => x.Character == 'A' && x.Count == 1);
            Assert.Contains(result, x => x.Character == 'B' && x.Count == 1);
        }

        [Fact]
        public void Count_ShouldHandleNumericCharacters()
        {
            var result = _service.Count("112233").ToList();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Count_ShouldHandleSpecialCharacters()
        {
            var result = _service.Count("!!@@##").ToList();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void Count_ShouldTreatUpperAndLowerCaseSeparately()
        {
            var result = _service.Count("aaAA").ToList();

            Assert.Equal(2, result.Count);

            Assert.Contains(result, x => x.Character == 'a' && x.Count == 1);
            Assert.Contains(result, x => x.Character == 'A' && x.Count == 1);
        }
    }
}
