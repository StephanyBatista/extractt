using System;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using FakeItEasy;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;

namespace Extractt.Test.Services
{
    public class ProcessDocumentTest
    {
        private readonly ProcessDocument _processDocument;
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly NewItemRequest _newItem;

        public ProcessDocumentTest()
        {
            _fileManager = A.Fake<FileManager>();
            _extractionManager = A.Fake<ExtractionManager>();
            var callback = A.Fake<Callback>();
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _processDocument = new ProcessDocument(_fileManager, _extractionManager, callback, _backgroundJobClient.Object);
            _newItem = new NewItemRequest { DocumentUrl = string.Empty, CallbackUrl = string.Empty, Identifier = string.Empty };
        }

        [Fact]
        public void Must_process_all_pages_from_document()
        {
            const string filePath = "file.pdf";
            const int numberOfPages = 2;
            A.CallTo(() => _fileManager.Download(_newItem.DocumentUrl)).Returns(filePath);
            A.CallTo(() => _fileManager.GetNumberOfPages(filePath)).Returns(numberOfPages);

            _processDocument.Process(_newItem).Wait();

            A.CallTo(() => _extractionManager.Extract(filePath, 1)).MustHaveHappened()
                .Then(A.CallTo(() => _extractionManager.Extract(filePath, 2)).MustHaveHappened());
        }

        [Fact]
        public void Must_enqueue_sucess_callback_after_process_document()
        {
            const string filePath = "file.pdf";
            const int numberOfPages = 1;
            const string textExpected = "text";
            A.CallTo(() => _fileManager.Download(_newItem.DocumentUrl)).Returns(filePath);
            A.CallTo(() => _fileManager.GetNumberOfPages(filePath)).Returns(numberOfPages);
            A.CallTo(() => _extractionManager.Extract(filePath, 1)).Returns(textExpected);

            _processDocument.Process(_newItem).Wait();

            _backgroundJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "Send" && (job.Args[0] as DocumentResultResponse).Success),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void Must_enqueue_fail_callback_when_has_exception()
        {
            A.CallTo(() => _fileManager.Download(_newItem.DocumentUrl)).Throws<Exception>();

            _processDocument.Process(_newItem).Wait();

            _backgroundJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "Send" && !(job.Args[0] as DocumentResultResponse).Success),
                It.IsAny<EnqueuedState>()));
        }
    }
}
