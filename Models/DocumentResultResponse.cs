using System.Collections.Generic;

namespace Extractt.Models
{
    public class DocumentResultResponse
    {
        public bool Sucess { get; set; }
        public string ErrorMessage { get; set; }
        public List<PageResultResponse> Pages { get; set; }

        public void AddProcessedPage(string text, int page)
        {
            if(Pages == null)
                Pages = new List<PageResultResponse>();

            Pages.Add(new PageResultResponse{ Text = text, Page = page });
        }
    }
}