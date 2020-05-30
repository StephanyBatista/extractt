using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Extractt.Models;
using Hangfire;
using Newtonsoft.Json;

namespace Extractt.Services
{
    public class Callback
    {
        private readonly IBackgroundJobClient _backgroundJobs;

        public Callback(IBackgroundJobClient backgroundJobs)
        {
            _backgroundJobs = backgroundJobs;
        }
        public async Task Send(DocumentResultResponse documentResult, NewItemRequest newItem)
        {
            using var client = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(documentResult);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(newItem.CallbackUrl, contentString).ConfigureAwait(false);
            if(response.StatusCode != HttpStatusCode.OK)
            {
                _backgroundJobs.Enqueue(() => Send(documentResult, newItem));
            }
        }
    }
}