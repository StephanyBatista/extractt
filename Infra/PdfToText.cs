using Extractt.Utils;

namespace Extractt.Infra
{
    public class PdfToText
    {
        public string Get(string filePath)
        {
            return $"pdftotext {filePath} -".Bash().Result;
        }
    }
}