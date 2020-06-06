using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Extractt.Web.Services;
using Newtonsoft.Json;

namespace Extractt.Web.Infra
{
    public class Cognitive : IExtractTextStrategy
    {
        private const string LanguageParameter = "language=pt";
        private static readonly string UriBase = EnvironmentVariables.CognitiveApi + "vision/v2.1/ocr";
        private static readonly string Uri = $"{UriBase}?{LanguageParameter}" ;
        private readonly FileManager _fileManager;

        public Cognitive(FileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task<string> Get(string imagePath)
        {
            return await MakeOCRRequest(imagePath).ConfigureAwait(false);
        }

        public virtual async Task<string> Exctract(string filePath, int page)
        {
            var imagePath = await _fileManager.GeneratePageInImage(filePath, page).ConfigureAwait(false);
            var text = await Get(imagePath).ConfigureAwait(false);
            await _fileManager.Delete(imagePath).ConfigureAwait(false);
            return text;
        }

        private async Task<HttpResponseMessage> GetResponse(string imagePath)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", EnvironmentVariables.CognitiveKey);
            var byteData = GetImageAsByteArray(imagePath);
            using var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            return await client.PostAsync(Uri, content).ConfigureAwait(false);
        }

        private async Task<string> MakeOCRRequest(string imagePath)
        {
            Console.WriteLine("Trying Cognitive");
            var response = await GetResponse(imagePath).ConfigureAwait(false);

            var contentString = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
            var resposta = JsonConvert.DeserializeObject<Response>(contentString);

            if(resposta == null || resposta.Regions == null)
                return string.Empty;

            var builder = new StringBuilder();
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
            using var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}