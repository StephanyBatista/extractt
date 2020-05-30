using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Extractt.Models;
using Newtonsoft.Json;

namespace Extractt.Services
{
    public class Callback
    {
        public async Task Send(List<ResultPdfDto> results, NewItemDto dto)
        {
            using var client = new HttpClient();
            var jsonContent = JsonConvert.SerializeObject(results);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(dto.CallbackUrl, contentString).ConfigureAwait(false);
            if(response.StatusCode != HttpStatusCode.OK)
                throw new System.Exception("Error to send");
        }
    }
}