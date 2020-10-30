using System;
using System.Linq;
using System.Threading.Tasks;
using Extractt.Web.Infra;
using Extractt.Web.Models;
using Extractt.Web.Utils;
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
            try
            {
                var filePath = await _fileManager.Download(newItem.Url).ConfigureAwait(false);
                var text = await $"pdftotext {filePath} -".Bash().ConfigureAwait(false);
                var numberOfPages = _fileManager.GetNumberOfPages(filePath);
                var documentResult = new DocumentResultResponse(numberOfPages, newItem.DocumentIdentifier, newItem.AccessKey);
                await RunInParallel(filePath, numberOfPages, documentResult).ConfigureAwait(false);

                Console.WriteLine($"Finishing document with URL {newItem.Url}");
                _fileManager.Delete(filePath);
                documentResult.Success = true;
                return documentResult;

            }
            catch (Exception ex) {
                return new DocumentResultResponse(ex.Message, newItem.DocumentIdentifier, newItem.AccessKey);
            }
        }

        private async Task RunInParallel(string filePath, int numberOfPages, DocumentResultResponse documentResult)
        {
            var numberOfItensInParallel = numberOfPages > EnvironmentVariables.NumberMaxDocumentsInParallel ? 
                EnvironmentVariables.NumberMaxDocumentsInParallel : numberOfPages;
            var skip = 0;
            var processed = 0;
            while (numberOfItensInParallel > 0)
            {
                var tasks = documentResult.Pages.Skip(skip).Take(numberOfItensInParallel).Select(page => ProcessPage(page, filePath));
                await Task.WhenAll(tasks).ConfigureAwait(false);
                processed += numberOfItensInParallel;
                skip += numberOfItensInParallel;
                numberOfItensInParallel = numberOfPages > processed + numberOfItensInParallel ?
                    numberOfItensInParallel :
                    numberOfPages - processed;
            }
        }

        public async Task ProcessPage(PageResultResponse page, string filePath)
        {
            page.Text = await _extractionManager.Extract(filePath, page.Page).ConfigureAwait(false);
        }
    }
}