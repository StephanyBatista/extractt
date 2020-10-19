using System.Collections.Generic;
using System.Threading.Tasks;
using Extractt.Web.Infra;

namespace Extractt.Web.Services
{
    public class ExtractionManager
    {
        private readonly List<IExtractTextStrategy> _extractionStrategies;

        public ExtractionManager(PdfToText pdfToText, Cognitive cognitive)
        {
            _extractionStrategies = new List<IExtractTextStrategy> { pdfToText, cognitive };
        }

        public virtual async Task<string> Extract(string filePath, int page)
        {
            foreach (var strategy in _extractionStrategies)
            {
                var text = await strategy.Exctract(filePath, page).ConfigureAwait(false);
                if (text != null)
                    return text;
            }

            return null;
        }
    }
}
