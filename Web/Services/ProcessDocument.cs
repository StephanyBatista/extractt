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
                Console.WriteLine("Initing documento extraction");
                var documentResult = new DocumentResultResponse();
                var filePath = await _fileManager.Download(newItem.DocumentUrl).ConfigureAwait(false);
                var numberOfPages = _fileManager.GetNumberOfPages(filePath);

                for(var page = 1; page <= numberOfPages; page++)
                {
                    var text = await _extractionManager.Extract(filePath, page).ConfigureAwait(false);
                    documentResult.AddProcessedPage(text, page);
                    Console.WriteLine("Page extracted");
                }

                Console.WriteLine("Finishing documento extraction");
                await _fileManager.Delete(filePath).ConfigureAwait(false);
                documentResult.Success = true;
                _backgroundJobs.Enqueue(() => _callback.Send(documentResult, newItem));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                var documentResult = new DocumentResultResponse { Success = false, ErrorMessage = ex.Message };
                _backgroundJobs.Enqueue(() => _callback.Send(documentResult, newItem));
            }
        }
    }
}