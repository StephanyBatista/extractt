using System;
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

        public async Task Process(NewItemRequest newItem)
        {
            try
            {
                var documentResult = new DocumentResultResponse();
                var filePath = _fileManager.Download(newItem.DocumentUrl);
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
                    documentResult.AddProcessedPage(text, page);
                    _fileManager.Delete(pagePath);
                }

                _fileManager.Delete(filePath);
                documentResult.Sucess = true;
                await _callback.Send(documentResult, newItem).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                var documentResult = new DocumentResultResponse { Sucess = false, ErrorMessage = ex.Message };
                await _callback.Send(documentResult, newItem).ConfigureAwait(false);
            }
        }
    }
}