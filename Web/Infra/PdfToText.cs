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
        
        public string Get(string filePath)
        {
            Console.WriteLine("Initing Pdf to text");
            return $"pdftotext {filePath} -".Bash().Result;
        }

        public virtual async Task<string> Exctract(string filePath, int page)
        {
            var pagePath = _fileManager.GeneratePage(filePath, page);
            var text = Get(pagePath);
            _fileManager.Delete(pagePath);
            if(string.IsNullOrEmpty(text) || text.Length < NumberMinOfCharacters) ;
                return null;
            return text;
        }
    }
}