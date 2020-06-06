using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Extractt.Web.Models;
using Newtonsoft.Json;

namespace Extractt.Web.Infra
{
    public class Callback
    {
        public async Task Send(DocumentResultResponse documentResult, NewItemRequest newItem)
        {
            documentResult.Identifier = newItem.Identifier;
            documentResult.AccessKey = newItem.AccessKey;
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            using var client = new HttpClient(handler);
            var jsonContent = JsonConvert.SerializeObject(documentResult);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(newItem.CallbackUrl, contentString).ConfigureAwait(false);
            if(response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error to callback");
        }
    }
}