using System;
using System.IO;
using System.Net;
using Extractt.Utils;
using iText.Kernel.Pdf;

namespace Extractt.Infra
{
    public class FileManager
    {
        public string Download(string url)
        {
            var localPath = $"{Directory.GetCurrentDirectory()}/{Guid.NewGuid().ToString().Substring(0,6)}.pdf";
            using var webClient = new WebClient();
            webClient.DownloadFile(new Uri(url), localPath);
            return localPath;
        }

        public int GetNumberOfPages(string pdfPath)
        {
            var pdfReader = new PdfReader(pdfPath);
            var document = new PdfDocument(pdfReader);
            return document.GetNumberOfPages();
        }

        public string GeneratePage(string pdfPath, int page)
        {
            var pageName = $"{Guid.NewGuid().ToString().Substring(0,6)}.pdf";
            var pagePath = Path.Combine(Directory.GetCurrentDirectory(), pageName);
            var command = $"qpdf {pdfPath} --pages {pdfPath} {page} -- {pagePath}";
            command.Bash().Wait();
            return pagePath;
        }

        public void Delete(string path)
        {
            System.IO.File.Delete(path);
        }

        public string GeneratePageInImage(string pdfPath, int page)
        {
            var imageName = Guid.NewGuid().ToString().Substring(0,6); 
            var localImagePath = Path.Combine(Directory.GetCurrentDirectory(), imageName);
            $"pdftoppm {pdfPath} {localImagePath} -jpeg -f {page} -singlefile".Bash().Wait();
            return $"{localImagePath}.jpg";
        }
    }
}