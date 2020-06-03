using System.Collections.Generic;

namespace Extractt.Web.Models
{
    public class DocumentResultResponse
    {
        public bool Success { get; set; }
        public string Identifier { get; set; }
        public string AccessKey { get; set; }
        public string ErrorMessage { get; set; }
        public List<PageResultResponse> Pages { get; set; }

        public void AddProcessedPage(string text, int page)
        {
            (Pages ??= new List<PageResultResponse>())
                .Add(new PageResultResponse{ Text = text, Page = page });
        }
    }
}