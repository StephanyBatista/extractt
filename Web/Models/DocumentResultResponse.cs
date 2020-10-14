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

        public DocumentResultResponse(int numberOfPages)
        {
            Pages = new List<PageResultResponse>();
            for(var i = 0; i < numberOfPages; i++) {
                Pages.Add(new PageResultResponse{Page = i+ 1});
            }
        }

        public DocumentResultResponse(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }
    }
}