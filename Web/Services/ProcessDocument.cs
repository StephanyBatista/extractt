using System;
using System.Threading.Tasks;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Hangfire;

namespace Extractt.Web.Services
{
    public class ProcessDocument
    {
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;
        private readonly Callback _callback;
        private readonly IBackgroundJobClient _backgroundJobs;

        public ProcessDocument(
            FileManager fileManager,
            ExtractionManager extractionManager,
            Callback callback,
            IBackgroundJobClient backgroundJobs)
        {
            _fileManager = fileManager;
            _extractionManager = extractionManager;
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
                    var text = await _extractionManager.Extract(filePath, page);
                    documentResult.AddProcessedPage(text, page);
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