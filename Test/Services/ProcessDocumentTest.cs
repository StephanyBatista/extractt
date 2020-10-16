using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Services;
using FakeItEasy;
using ExpectedObjects;
using Xunit;
using Web.Infra;

namespace Extractt.Test.Services
{
    public class ProcessDocumentTest
    {
        private readonly ProcessDocument _processDocument;
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;
        private readonly NewFileToProcess _newFileToProcess;
        private readonly Callback _callback;

        public ProcessDocumentTest()
        {
            _fileManager = A.Fake<FileManager>();
            _extractionManager = A.Fake<ExtractionManager>();
            _callback = A.Fake<Callback>();
            _processDocument = new ProcessDocument(_fileManager, _extractionManager, _callback);
            _newFileToProcess = new NewFileToProcess { Url = string.Empty };
        }

        [Fact]
        public void Must_process_all_pages_from_document()
        {
            const string filePath = "file.pdf";
            const int numberOfPages = 2;
            const string pageText1 = "Text 1";
            const string pageText2 = "Text 2";
            A.CallTo(() => _fileManager.Download(_newFileToProcess.Url)).Returns(filePath);
            A.CallTo(() => _fileManager.GetNumberOfPages(filePath)).Returns(numberOfPages);
            A.CallTo(() => _extractionManager.Extract(filePath, 1)).Returns(pageText1);
            A.CallTo(() => _extractionManager.Extract(filePath, 2)).Returns(pageText2);

            _processDocument.Process(_newFileToProcess).Wait();

            A.CallTo(() => _callback.Send(
                A<DocumentResultResponse>.That.Matches(d => d.Pages.Count == numberOfPages),
                _newFileToProcess.CallbackUrl));
            A.CallTo(() => _callback.Send(
                A<DocumentResultResponse>.That.Matches(d => d.Pages[0].Text == pageText1),
                _newFileToProcess.CallbackUrl));
            A.CallTo(() => _callback.Send(
                A<DocumentResultResponse>.That.Matches(d => d.Pages[1].Text == pageText2),
                _newFileToProcess.CallbackUrl));
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
