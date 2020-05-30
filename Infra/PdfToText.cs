using System;
using Extractt.Utils;

namespace Extractt.Infra
{
    public class PdfToText
    {
        public string Get(string filePath)
        {
            Console.WriteLine("Initing Pdf to text");
            return $"pdftotext {filePath} -".Bash().Result;
        }
    }
}