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

        public Template ? Template { get; set; }
        public  string Title { get; set; }  
        public string ? Description { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
