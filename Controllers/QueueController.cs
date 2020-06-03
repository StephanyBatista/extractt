using Extractt.Models;
using Extractt.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Extractt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly ProcessDocument _processDocument;
        private readonly IBackgroundJobClient _backgroundJobs;

        public QueueController(
            ILogger<QueueController> logger,
            ProcessDocument processDocument,
            IBackgroundJobClient backgroundJobs)
        {
            _logger = logger;
            _processDocument = processDocument;
            _backgroundJobs = backgroundJobs;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post(NewItemRequest dto)
        {
            _logger.LogInformation("New request");

            _backgroundJobs.Enqueue(() => _processDocument.Process(dto));

            return Ok();
        }
    }
}
