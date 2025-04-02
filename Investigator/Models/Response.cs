using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }
        [Required]
        public int FormId { get; set; }

        [ForeignKey(nameof(FormId))]
        [ValidateNever]
        public Form? Form { get; set; }
        [Required]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        [ValidateNever]
        public Question? Question { get; set; }
        [Required]
        public string Answer { get; set; } = string.Empty;

    }
}
