using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Extractt.Web.Utils;
using iText.Kernel.Pdf;

namespace Extractt.Web.Infra
{
    public class FileManager
    {
        private static readonly HttpClient client = new HttpClient();

        public virtual async Task<string> Download(string url)
        {
            Console.Write($"Downloading file {url}");
            using var result = await client.GetAsync(url).ConfigureAwait(false);
            if(!result.IsSuccessStatusCode)
                throw new Exception("Error to download file");
            var contentStream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var localPath = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid().ToString().Substring(0,6)}.pdf";
            using var file = File.Create(localPath);
            await contentStream.CopyToAsync(file).ConfigureAwait(false);
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

        public virtual void Delete(string path)
        {
            File.Delete(path);
        }

        public async Task<string> GeneratePageInImage(string pdfPath, int page)
        {
            var imageName = Guid.NewGuid().ToString().Substring(0,6);
            var localImagePath = Path.Combine(Directory.GetCurrentDirectory(), imageName);
            await $"pdftoppm {pdfPath} {localImagePath} -jpeg -f {page} -singlefile -r 80".Bash().ConfigureAwait(false);
            return $"{localImagePath}.jpg";
        }
    }
}