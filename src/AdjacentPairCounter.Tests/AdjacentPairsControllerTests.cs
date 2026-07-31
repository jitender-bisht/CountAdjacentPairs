using AdjacentPairCounter.Application.DTOs;
using AdjacentPairCounter.Application.Interfaces;
using AdjacentPairCounter.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdjacentPairCounter.Tests
{
    /// <summary>
    /// Unit tests for <see cref="AdjacentPairsController"/>. The service is mocked so these tests verify
    /// HTTP wiring (200 OK, response shape, delegation to the service) rather than the counting algorithm itself.
    /// </summary>
    public class AdjacentPairsControllerTests
    {
        private readonly Mock<IAdjacentPairService> _serviceMock;
        private readonly AdjacentPairsController _controller;

        public AdjacentPairsControllerTests()
        {
            _serviceMock = new Mock<IAdjacentPairService>();
            _controller = new AdjacentPairsController(_serviceMock.Object);
        }
        [Fact]
        public void Count_ShouldReturnOk_WithAdjacentPairs()
        {
            // Arrange
            var request = new AdjacentPairRequest
            {
                Input = "AABBB"
            };

            var response = new List<AdjacentPairResponse>
            {
                new() { Character = 'A', Count = 1 },
                new() { Character = 'B', Count = 1 }
            };

            _serviceMock
                .Setup(x => x.Count(request.Input))
                .Returns(response);

            // Act
            var result = _controller.Count(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<AdjacentPairResponse>>(okResult.Value);

            Assert.Equal(2, value.Count());
        }

        [Fact]
        public void Count_ShouldReturnOk_WhenNoPairsFound()
        {
            // Arrange
            var request = new AdjacentPairRequest
            {
                Input = "ABCDEF"
            };

            _serviceMock
                .Setup(x => x.Count(request.Input))
                .Returns(new List<AdjacentPairResponse>());

            // Act
            var result = _controller.Count(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsAssignableFrom<IEnumerable<AdjacentPairResponse>>(okResult.Value);

            Assert.Empty(value);
        }

        [Fact]
        public void Count_ShouldCallServiceOnce()
        {
            // Arrange
            var request = new AdjacentPairRequest
            {
                Input = "AABB"
            };

            _serviceMock
                .Setup(x => x.Count(request.Input))
                .Returns(new List<AdjacentPairResponse>());

            // Act
            _controller.Count(request);

            // Assert
            _serviceMock.Verify(
                x => x.Count(request.Input),
                Times.Once);
        }
    }
}