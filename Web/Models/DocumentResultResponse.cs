using System.Collections.Generic;

namespace Extractt.Web.Models
{
    public class DocumentResultResponse
    {
        public bool Success { get; set; }
        public string DocumentIdentifier { get; set; }
        public string AccessKey { get; set; }
        public string ErrorMessage { get; set; }
        public List<PageResultResponse> Pages { get; set; }

        public DocumentResultResponse(int numberOfPages, string documentIdentifier, string accessKey)
        {
            Pages = new List<PageResultResponse>();
            for(var i = 0; i < numberOfPages; i++) {
                Pages.Add(new PageResultResponse{Page = i+ 1});
            }
            DocumentIdentifier = documentIdentifier;
            AccessKey = accessKey;
        }

        public DocumentResultResponse(string errorMessage, string documentIdentifier, string accessKey)
        {
            Success = false;
            ErrorMessage = errorMessage;
            DocumentIdentifier = documentIdentifier;
            AccessKey = accessKey;
        }
    }
}