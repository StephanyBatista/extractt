using System;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Extractt.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ProcessDocument _processDocument;
        private readonly IBackgroundJobClient _backgroundJobs;

        public QueueController(
            ProcessDocument processDocument,
            IBackgroundJobClient backgroundJobs)
        {
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
            var result = ValidateParameters(dto);
            if (!(result is OkResult))
                return result;

            _backgroundJobs.Enqueue(() => _processDocument.Process(dto));

            return Ok();
        }

        private IActionResult ValidateParameters(NewItemRequest dto)
        {
            if (dto == null)
                return BadRequest("Model is null");

            if (dto.DocumentUrl == null)
                return BadRequest("Document url is null");

            if (dto.CallbackUrl == null)
                return BadRequest("Callback url is null");

            if (dto.AccessKey == null)
                return BadRequest("Access key is null");

            if (dto.AccessKey != EnvironmentVariables.AccessKey)
                return BadRequest("Access key is invalid");

            return Ok();
        }
    }
}
