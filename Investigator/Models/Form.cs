using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }
        public int? TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        [ValidateNever]
        public Template? Template { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public string? CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [ValidateNever]
        public ApplicationUser? Creator { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }

}
