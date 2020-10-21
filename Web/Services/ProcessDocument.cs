using System;
using System.Linq;
using System.Threading.Tasks;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Web.Infra;

namespace Extractt.Web.Services
{
    public class ProcessDocument
    {
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;
        private readonly Callback _callback;

        public ProcessDocument(
            FileManager fileManager,
            ExtractionManager extractionManager,
            Callback callback)
        {
            _fileManager = fileManager;
            _extractionManager = extractionManager;
            _callback = callback;
        }

        public async Task Process(NewFileToProcess newItem)
        {
            var result = await ExtractOcr(newItem).ConfigureAwait(false);
            await _callback.Send(result, newItem.CallbackUrl).ConfigureAwait(false);
        }

        private async Task<DocumentResultResponse> ExtractOcr(NewFileToProcess newItem)
        {
            try {
                var filePath = await _fileManager.Download(newItem.Url).ConfigureAwait(false);
                var numberOfPages = _fileManager.GetNumberOfPages(filePath);
                var documentResult = new DocumentResultResponse(numberOfPages, newItem.DocumentIdentifier, newItem.AccessKey);

                var tasks = documentResult.Pages.Select(page => ProcessPage(page, filePath));
                await Task.WhenAll(tasks).ConfigureAwait(false);

                Console.WriteLine($"Finishing document with URL {newItem.Url}");
                await _fileManager.Delete(filePath).ConfigureAwait(false);
                documentResult.Success = true;
                return documentResult;

            } catch(Exception ex) {
                return new DocumentResultResponse(ex.Message, newItem.DocumentIdentifier, newItem.AccessKey);
            }
        }

        public async Task ProcessPage(PageResultResponse page, string filePath)
        {
            page.Text = await _extractionManager.Extract(filePath, page.Page).ConfigureAwait(false);
        }
    }
}