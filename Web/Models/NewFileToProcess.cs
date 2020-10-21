using System.ComponentModel.DataAnnotations;

namespace Extractt.Web.Models
{
    public class NewFileToProcess
    {
        [Required]
        public string Url { get; set; }

        [Required]
        public string AccessKey { get; set; }

        [Required]
        public string CallbackUrl  { get; set; }

        [Required]
        public string DocumentIdentifier  { get; set; }
    }
}