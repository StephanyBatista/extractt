using System.ComponentModel.DataAnnotations;

namespace Extractt.Web.Models
{
    public class NewItemRequest
    {
        [Required]
        public string DocumentUrl { get; set; }
        [Required]
        public string CallbackUrl { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string AccessKey { get; set; }
    }
}