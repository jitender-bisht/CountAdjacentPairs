using AdjacentPairCounter.Application.DTOs;
using AdjacentPairCounter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdjacentPairCounter.Api.Controllers
{
    /// <summary>
    /// Exposes HTTP endpoints for counting adjacent (consecutive, non-overlapping) duplicate characters in a string.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class AdjacentPairsController : ControllerBase
    {
        private readonly IAdjacentPairService _service;

        public AdjacentPairsController(IAdjacentPairService service)
        {
            _service = service;
        }

        // POST api/adjacentpairs/count
        [HttpPost("count")]
        public IActionResult Count(AdjacentPairRequest request)
        {
            var result = _service.Count(request.Input);

            return Ok(result);
        }
    }
}
