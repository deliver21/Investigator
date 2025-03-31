using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public int? FormId { get; set; }
        public Form ? Form { get; set; }
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; }
        public int Order { get; set; }
        public bool IsOptional { get; set; } = false;
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
