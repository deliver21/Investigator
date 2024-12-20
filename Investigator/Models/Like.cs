using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }

        [Required]
        public int TemplateId { get; set; }
        [ForeignKey(nameof(TemplateId))]
        [ValidateNever]
        public Template Template { get; set; }

        [Required]
        public string LikerId { get; set; }
        [ForeignKey(nameof(LikerId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
