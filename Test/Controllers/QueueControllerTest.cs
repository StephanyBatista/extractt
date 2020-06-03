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
    public class QueuerControllerTest
    {
        private const int OkStatusCode = 200;
        private const int BadRequestStatusCode = 400;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly QueueController _controller;
        private readonly NewItemRequest _newItem;

        public QueuerControllerTest()
        {
            EnvironmentVariables.AccessKey = "AACC$%";
            var processDocument = A.Fake<ProcessDocument>();
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _controller = new QueueController(processDocument, _backgroundJobClient.Object);
            _newItem = new NewItemRequest { DocumentUrl = "Url", CallbackUrl = "Url", AccessKey = EnvironmentVariables.AccessKey };
        }

        [Fact]
        public void Must_validate_parameter_model()
        {
            const string messageError = "Model is null";

            var result = _controller.Post(null) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
            Assert.Equal(messageError, result.Value.ToString());
        }

        [Fact]
        public void Must_validate_parameter_document_url()
        {
            const string messageError = "Document url is null";

            var result = _controller.Post(new NewItemRequest()) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
            Assert.Equal(messageError, result.Value.ToString());
        }

        [Fact]
        public void Must_validate_parameter_callback_url()
        {
            const string messageError = "Callback url is null";

            var result = _controller.Post(new NewItemRequest { DocumentUrl = "url" }) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
            Assert.Equal(messageError, result.Value.ToString());
        }

        [Fact]
        public void Must_validate_parameter_access_key()
        {
            const string messageError = "Access key is null";

            var result = _controller.Post(new NewItemRequest { DocumentUrl = "url", CallbackUrl = "url"}) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
            Assert.Equal(messageError, result.Value.ToString());
        }

        [Fact]
        public void Must_validate_access_key()
        {
            const string accessKeyInvalid = "XPTO";
            const string messageError = "Access key is invalid";

            var result = _controller.Post(new NewItemRequest { DocumentUrl = "url", CallbackUrl = "url", AccessKey = accessKeyInvalid }) as BadRequestObjectResult;

            Assert.Equal(BadRequestStatusCode, result.StatusCode);
            Assert.Equal(messageError, result.Value.ToString());
        }

        [Fact]
        public void Must_enqueue_new_item()
        {
            _controller.Post(_newItem);

            _backgroundJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "Process" && job.Args[0] == _newItem),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Must_return_ok_after_enqueue()
        {
            var result = _controller.Post(_newItem) as OkResult;

            Assert.Equal(OkStatusCode, result.StatusCode);
        }
    }
}
