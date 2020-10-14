using System;
using System.Linq;
using System.Threading.Tasks;
using Extractt.Web.Infra;
using Extractt.Web.Models;

namespace Extractt.Web.Services
{
    public class ProcessDocument
    {
        private readonly FileManager _fileManager;
        private readonly ExtractionManager _extractionManager;

        public ProcessDocument(
            FileManager fileManager,
            ExtractionManager extractionManager)
        {
            _fileManager = fileManager;
            _extractionManager = extractionManager;
        }

        public async Task<DocumentResultResponse> Process(NewFileToProcess newItem)
        {
            try{
                var filePath = await _fileManager.Download(newItem.Url).ConfigureAwait(false);
                var numberOfPages = _fileManager.GetNumberOfPages(filePath);
                var documentResult = new DocumentResultResponse(numberOfPages);

                var tasks = documentResult.Pages.Select(page => ProcessPage(page, filePath));
                await Task.WhenAll(tasks).ConfigureAwait(false);

                Console.WriteLine($"Finishing document with URL {newItem.Url}");
                await _fileManager.Delete(filePath).ConfigureAwait(false);
                documentResult.Success = true;
                return documentResult;

            }catch(Exception ex) {
                return new DocumentResultResponse(ex.Message);
            }
        }

        public async Task ProcessPage(PageResultResponse page, string filePath)
        {
            page.Text = await _extractionManager.Extract(filePath, page.Page).ConfigureAwait(false);
        }
    }
}