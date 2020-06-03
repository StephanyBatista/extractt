using System.Threading.Tasks;

namespace Extractt.Web.Services
{
    public interface IExtractTextStrategy
    {
        Task<string> Exctract(string filePath, int page);
    }
}
