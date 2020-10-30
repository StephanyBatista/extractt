using System;
using System.Threading.Tasks;
using Extractt.Web.Services;
using Extractt.Web.Utils;

namespace Extractt.Web.Infra
{
    public class PdfToText : IExtractTextStrategy
    {
        private const int NumberMinOfCharacters = 12;
        private readonly FileManager _fileManager;

        public PdfToText(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task<string> Get(string filePath)
        {
            return await $"pdftotext {filePath} -".Bash().ConfigureAwait(false);
        }

        public virtual async Task<string> Exctract(string filePath, int page)
        {
            Console.WriteLine($"Trying PdfToText page {page} from {filePath}");
            var initialDate = DateTime.Now;
            var pagePath = await _fileManager.GeneratePage(filePath, page).ConfigureAwait(false);
            var text = await Get(pagePath).ConfigureAwait(false);
            _fileManager.Delete(pagePath);
            if(string.IsNullOrEmpty(text) || text.Length < NumberMinOfCharacters)
                return null;
            Console.WriteLine($"Success PdfToText page {page} from {filePath} on {DateTime.Now.Subtract(initialDate).Seconds}s");
            return text;
        }
    }
}