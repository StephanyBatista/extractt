using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Extractt.Web.Utils;
using iText.Kernel.Pdf;

namespace Extractt.Web.Infra
{
    public class FileManager
    {
        private static HttpClient client = new HttpClient();
        
        public virtual async Task<string> Download(string url)
        {
            Console.Write($"Downloading file {url}");
            var localPath = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid().ToString().Substring(0,6)}.pdf";
            using var result = await client.GetAsync(url);
            
            using var file = System.IO.File.Create(localPath);
            var contentStream = await result.Content.ReadAsStreamAsync();
            await contentStream.CopyToAsync(file);
            return localPath;
        }

        public virtual int GetNumberOfPages(string pdfPath)
        {
            using var pdfReader = new PdfReader(pdfPath);
            using var document = new PdfDocument(pdfReader);
            return document.GetNumberOfPages();
        }

        public async Task<string> GeneratePage(string pdfPath, int page)
        {
            var pageName = $"{Guid.NewGuid().ToString().Substring(0,6)}.pdf";
            var pagePath = Path.Combine(Directory.GetCurrentDirectory(), pageName);
            var command = $"qpdf {pdfPath} --pages {pdfPath} {page} -- {pagePath}";
            await command.Bash().ConfigureAwait(false);
            return pagePath;
        }

        public virtual async Task Delete(string path)
        {
            await Task.Run(() => System.IO.File.Delete(path)).ConfigureAwait(false);
        }

        public async Task<string> GeneratePageInImage(string pdfPath, int page)
        {
            var imageName = Guid.NewGuid().ToString().Substring(0,6); 
            var localImagePath = Path.Combine(Directory.GetCurrentDirectory(), imageName);
            await $"pdftoppm {pdfPath} {localImagePath} -jpeg -f {page} -singlefile".Bash().ConfigureAwait(false);
            return $"{localImagePath}.jpg";
        }
    }
}