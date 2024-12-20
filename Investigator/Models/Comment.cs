using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int TemplateId { get; set; }
        [ForeignKey(nameof(TemplateId))]
        [ValidateNever]
        public Template Template { get; set; }

        [Required]        
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.Now;
    }
}
