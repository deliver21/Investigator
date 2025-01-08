using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Form
    {
        [Key]
        public int FormId { get; set; }
        [Required]
        public int ? TemplateId { get; set; }

        [ForeignKey(nameof(TemplateId))]
        [ValidateNever]
        public Template ? Template { get; set; }
        public  string Title { get; set; }  
        public string Description { get; set; }
        [Required]
        public string CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [ValidateNever]
        public ApplicationUser Creator { get; set; }
        public string ? FillerId { get; set; }

        [ForeignKey(nameof(FillerId))]
        [ValidateNever]
        public ApplicationUser Filler { get; set; }
        public DateTime SubmissionDate { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
