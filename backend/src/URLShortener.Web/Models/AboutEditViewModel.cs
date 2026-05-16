using System.ComponentModel.DataAnnotations;

namespace URLShortener.Web.Models
{
    public class AboutEditViewModel
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
