using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Extractt.Infra
{
    public class Cognitive
    {
        public async Task<string> Get(string imagePath)
        {
            return await MakeOCRRequest(imagePath).ConfigureAwait(false);
        }

        private async Task<string> MakeOCRRequest(string imagePath)
        {
            Console.WriteLine("Initing Microsoft Cognitive");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("COGNITIVE_KEY"));
            var uriBase = Environment.GetEnvironmentVariable("COGNITIVE_API") + "vision/v2.1/ocr";
            const string requestParameters = "language=pt";
            var uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(imagePath);

            // Add the byte array as an octet stream to the request body.
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content).ConfigureAwait(false);
            }

            string contentString = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
            var resposta = JsonConvert.DeserializeObject<Response>(contentString);

            if(resposta == null || resposta.Regions == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (var region in resposta.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        builder.Append(word.Text).Append(" ");
                    }
                }
            }
            return builder.ToString();
        }

        private byte[] GetImageAsByteArray(string imagePath)
        {
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}