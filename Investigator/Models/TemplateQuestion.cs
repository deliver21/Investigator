using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Investigator.Models
{
     public class TemplateQuestion
     {       
         [Key]
         public int TemplateQuestionId { get; set; }
         [Required]
         public int? TemplateId { get; set; }
         [ForeignKey(nameof(TemplateId))]
         [ValidateNever]
         public Template? Template { get; set; }
         [Required]
         public string Text { get; set; } = string.Empty;
         [Required]
         public string Type { get; set; }
         public int Order { get; set; }
         public bool IsOptional { get; set; } = false;
     }    
}
