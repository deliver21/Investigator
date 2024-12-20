using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Investigator.Models
{
    [Index(nameof(TagName))]
    public class TemplateTag
    {
        [Key]
        public int TagId { get; set; }
        [Required]
        public string TagName { get; set; }
    }    
}
