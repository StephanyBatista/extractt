using Extractt.Web.Controllers;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using FakeItEasy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Extractt.Test.Controllers
{
    public class ProcessControllerTest
    {
        private const int OkStatusCode = 200;
        private const int BadRequestStatusCode = 400;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly ProcessController _controller;

        public ProcessControllerTest()
        {
            EnvironmentVariables.AccessKey = "AACC$%";
            var processDocument = A.Fake<ProcessDocument>();
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _controller = new ProcessController(processDocument, _backgroundJobClient.Object);
        }

        [Fact]
        public void Must_validate_parameters()
        {
            _controller.ModelState.AddModelError("Url", "Missing parameter");

            var result = _controller.Post(null) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
        }

        [Fact]
        public void Must_enqueue_new_item()
        {
            var newItem = new NewFileToProcess {
                Url = "Url", CallbackUrl = "Url", AccessKey = EnvironmentVariables.AccessKey, DocumentIdentifier = "XPTO" };

            _controller.Post(newItem);

            _backgroundJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "Process" && job.Args[0] == newItem),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Must_return_ok_after_enqueue()
        {
            var newItem = new NewFileToProcess {
                Url = "Url", CallbackUrl = "Url", AccessKey = EnvironmentVariables.AccessKey, DocumentIdentifier = "XPTO" };

            var result = _controller.Post(newItem) as OkResult;

            Assert.Equal(OkStatusCode, result.StatusCode);
        }
    }
}
