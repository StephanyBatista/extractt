using Extractt.Web.Models;
using Extractt.Web.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Extractt.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly ProcessDocument _processDocument;
        private readonly IBackgroundJobClient _backgroundJobs;
        public ProcessController(ProcessDocument processDocument,
            IBackgroundJobClient backgroundJobs)
        {
            _processDocument = processDocument;
            _backgroundJobs = backgroundJobs;
        }

        [HttpPost]
        public IActionResult Post(NewFileToProcess dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Missing parameters");

            _backgroundJobs.Enqueue(() => _processDocument.Process(dto));

            return Ok();
        }
    }
}
