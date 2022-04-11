using API.Services.Interface;
using Commons.Models.Exception;
using Microsoft.AspNetCore.Mvc;

namespace RidelyRecruitment.Controllers
{
    [ApiController]
    [Route("visitors")]
    public class VisitorsController : ControllerBase
    {
        private readonly ILogger<VisitorsController> _logger;
        private readonly IVisitorsService _visitorsService;

        public VisitorsController(ILogger<VisitorsController> logger, IVisitorsService visitorsService)
        {
            _logger = logger;
            _visitorsService = visitorsService;
        }

        [HttpGet("by-country")]
        public async Task<IActionResult> GetVisitorsCounts()
        {
            return Ok(await _visitorsService.GetVisitorsCounts());
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetProcessedFilesCount()
        {
            return Ok(await _visitorsService.GetProcessedFilesCount());
        }

        [HttpPost("visit")]
        public async Task<IActionResult> UploadVisits(IEnumerable<IFormFile> files)
        {
            await _visitorsService.UploadVisits(files);

            return Ok();
        }
    }
}