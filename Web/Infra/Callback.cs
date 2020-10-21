using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Extractt.Web.Models;
using Newtonsoft.Json;

namespace Web.Infra
{
    public class Callback
    {
        private static readonly HttpClient client = new HttpClient();

        public virtual async Task Send(DocumentResultResponse documentResult, string callbackUrl)
        {
            Console.WriteLine($"Url to response client: {callbackUrl}");
            var resultAtJson = JsonConvert.SerializeObject(documentResult);
            var content = new StringContent(resultAtJson, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(callbackUrl, content).ConfigureAwait(false);
            if(response.StatusCode != HttpStatusCode.OK) {
                var clientResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new Exception($"Error to response client: {clientResponse}");
            }
        }
    }
}