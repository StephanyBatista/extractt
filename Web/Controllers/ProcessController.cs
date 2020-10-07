using System.Threading.Tasks;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Extractt.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly ProcessDocument _processDocument;
        public ProcessController(ProcessDocument processDocument)
        {
            _processDocument = processDocument;
        }

        [HttpPost]
        public async Task<IActionResult> Get(NewFileToProcess dto)
        {
            var result = await _processDocument.Process(dto).ConfigureAwait(false);
            return Ok(result);
        }

        private IActionResult ValidateParameters(NewFileToProcess dto)
        {
            if (dto == null)
                return BadRequest("Model is null");

            if (string.IsNullOrEmpty(dto.Url))
                return BadRequest("URL is invalid");

            if (dto.AccessKey == null)
                return BadRequest("Access key is null");

            if (dto.AccessKey != EnvironmentVariables.AccessKey)
                return BadRequest("Access key is invalid");

            return Ok();
        }
    }
}
