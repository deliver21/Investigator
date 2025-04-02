using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class FormFiller
    {
        [Key]
        public int FormFillerId { get; set; } 
        public string? Filler { get; set;}
        [ForeignKey(nameof(Filler))]
        [ValidateNever]
        public ApplicationUser? ApllicationUser { get; set; }  
        public int FormId { get; set; }
        [ForeignKey(nameof(FormId))]
        [ValidateNever]
        public Form? Form { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        public ICollection<Response> Responses { get; set; } = new List<Response>();
    }
}
