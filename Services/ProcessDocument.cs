using System;
using System.Threading.Tasks;
using Extractt.Infra;
using Extractt.Models;
using Hangfire;

namespace Extractt.Services
{
    public class ProcessDocument
    {
        private readonly FileManager _fileManager;
        private readonly PdfToText _pdfToText;
        private readonly Cognitive _cognitive;
        private readonly Callback _callback;
        private readonly IBackgroundJobClient _backgroundJobs;

        public ProcessDocument(
            FileManager fileManager,
            PdfToText pdfToText,
            Cognitive cognitive,
            Callback callback,
            IBackgroundJobClient backgroundJobs)
        {
            _fileManager = fileManager;
            _pdfToText = pdfToText;
            _cognitive = cognitive;
            _callback = callback;
            _backgroundJobs = backgroundJobs;
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
                    if(text == null)
                    {
                        var imagePath = _fileManager.GeneratePageInImage(filePath, page);
                        text = await _cognitive.Get(imagePath).ConfigureAwait(false);
                        _fileManager.Delete(imagePath);
                    }
                    documentResult.AddProcessedPage(text, page);
                    _fileManager.Delete(pagePath);
                }

                _fileManager.Delete(filePath);
                documentResult.Success = true;
                _backgroundJobs.Enqueue(() => _callback.Send(documentResult, newItem));
            }
            catch(Exception ex)
            {
                var documentResult = new DocumentResultResponse { Success = false, ErrorMessage = ex.Message };
                _backgroundJobs.Enqueue(() => _callback.Send(documentResult, newItem));
            }
        }
    }
}