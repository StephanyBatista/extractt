using System;
using System.Threading.Tasks;
using Extractt.Web.Services;
using Extractt.Web.Utils;

namespace Extractt.Web.Infra
{
    public class PdfToText : IExtractTextStrategy
    {
        private const int NumberMinOfCharacters = 24;
        private readonly FileManager _fileManager;

        public PdfToText(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task<string> Get(string filePath)
        {
            Console.WriteLine("Trying PdfToText");
            return await $"pdftotext {filePath} -".Bash().ConfigureAwait(false);
        }

        public virtual async Task<string> Exctract(string filePath, int page)
        {
            var pagePath = await _fileManager.GeneratePage(filePath, page).ConfigureAwait(false);
            var text = await Get(pagePath).ConfigureAwait(false);
            await _fileManager.Delete(pagePath).ConfigureAwait(false);
            if(string.IsNullOrEmpty(text) || text.Length < NumberMinOfCharacters)
                return null;
            return text;
        }
    }
}