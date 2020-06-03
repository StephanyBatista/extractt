using Extractt.Web.Infra;
using Extractt.Web.Services;
using FakeItEasy;
using Xunit;

namespace Extractt.Test.Services
{
    public class ExtractionManagerTest
    {
        private const string FilePath = "/file.pdf";
        private const int Page = 1;
        private readonly PdfToText _pdfToText;
        private readonly Cognitive _cognitive;
        private readonly ExtractionManager _manager;

        public ExtractionManagerTest()
        {
            _pdfToText = A.Fake<PdfToText>();
            _cognitive = A.Fake<Cognitive>();

            _manager = new ExtractionManager(_pdfToText, _cognitive);
        }

        [Fact]
        public void Must_pdf_to_text_be_called_first()
        {
            const string textExpected = "Text";
            A.CallTo(() => _pdfToText.Exctract(FilePath, Page)).Returns(textExpected);

            var text = _manager.Extract(FilePath, Page).Result;

            Assert.Equal(textExpected, text);
        }

        [Fact]
        public void Must_cognitive_be_called_after_pdf_to_text()
        {
            const string textExpected = "Text";
            A.CallTo(() => _pdfToText.Exctract(FilePath, Page)).Returns((string)null);
            A.CallTo(() => _cognitive.Exctract(FilePath, Page)).Returns(textExpected);

            var text = _manager.Extract(FilePath, Page).Result;

            Assert.Equal(textExpected, text);
        }
    }
}
