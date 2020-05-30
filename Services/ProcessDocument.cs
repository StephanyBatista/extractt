using System.Collections.Generic;
using System.Threading.Tasks;
using Extractt.Infra;
using Extractt.Models;

namespace Extractt.Services
{
    public class ProcessDocument
    {
        private readonly FileManager _fileManager;
        private readonly PdfToText _pdfToText;
        private readonly Cognitive _cognitive;
        private readonly Callback _callback;

        public ProcessDocument(FileManager fileManager, PdfToText pdfToText, Cognitive cognitive, Callback callback)
        {
            _fileManager = fileManager;
            _pdfToText = pdfToText;
            _cognitive = cognitive;
            _callback = callback;
        }

        public async Task Process(NewItemDto dto)
        {
            var result = new List<ResultPdfDto>();
            var filePath = _fileManager.Download(dto.DocumentUrl);
            var numberOfPages = _fileManager.GetNumberOfPages(filePath);

            for(var page = 1; page <= numberOfPages; page++)
            {
                var pagePath = _fileManager.GeneratePage(filePath, page);
                var text = _pdfToText.Get(pagePath);
                if(string.IsNullOrEmpty(text) || text.Length < 256)
                {
                    var imagePath = _fileManager.GeneratePageInImage(filePath, page);
                    text = await _cognitive.Get(imagePath).ConfigureAwait(false);
                    _fileManager.Delete(imagePath);
                }
                result.Add(new ResultPdfDto{ Text = text, Page = page});
                _fileManager.Delete(pagePath);
            }

            _fileManager.Delete(filePath);
            await _callback.Send(result, dto).ConfigureAwait(false);
        }
    }
}