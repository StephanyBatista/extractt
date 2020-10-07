using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using FakeItEasy;
using ExpectedObjects;
using Xunit;

namespace Extractt.Test.Services
{
    public class ProcessDocumentTest
    {
        private readonly ProcessDocument _processDocument;
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;
        private readonly NewFileToProcess _newFileToProcess;

        public ProcessDocumentTest()
        {
            _fileManager = A.Fake<FileManager>();
            _extractionManager = A.Fake<ExtractionManager>();
            _processDocument = new ProcessDocument(_fileManager, _extractionManager);
            _newFileToProcess = new NewFileToProcess { Url = string.Empty };
        }

        [Fact]
        public void Must_process_all_pages_from_document()
        {
            const string filePath = "file.pdf";
            const int numberOfPages = 2;
            var resultExpected = new {
                Success = true,
                Pages = new[] {
                    new {Page = 1, Text = "Text 1"},
                    new {Page = 2, Text = "Text 2"}
                }
            };
            A.CallTo(() => _fileManager.Download(_newFileToProcess.Url)).Returns(filePath);
            A.CallTo(() => _fileManager.GetNumberOfPages(filePath)).Returns(numberOfPages);
            A.CallTo(() => _extractionManager.Extract(filePath, resultExpected.Pages[0].Page))
                .Returns(resultExpected.Pages[0].Text);
            A.CallTo(() => _extractionManager.Extract(filePath, resultExpected.Pages[1].Page))
                .Returns(resultExpected.Pages[1].Text);

            var result = _processDocument.Process(_newFileToProcess).Result;

            resultExpected.ToExpectedObject().ShouldMatch(result);
        }

        [Fact]
        public void Must_delete_file_downloaded()
        {
            const string filePath = "file.pdf";
            const int numberOfPages = 2;
            A.CallTo(() => _fileManager.Download(_newFileToProcess.Url)).Returns(filePath);
            A.CallTo(() => _fileManager.GetNumberOfPages(filePath)).Returns(numberOfPages);

            _processDocument.Process(_newFileToProcess).Wait();

            A.CallTo(() => _fileManager.Delete(filePath)).MustHaveHappened();
        }
    }
}
