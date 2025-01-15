using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    public class JiraTicket
    {
        [Key]
        public int TicketId { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Priority { get; set; }
        public string? TemplateTitle { get; set; }
        public string PageLink { get; set; }
        [Required]
        public string Status { get; set; } = "Opened";
        [Required]
        public string ? CreatedByUserId { get; set; }
        [ValidateNever]

        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser CreatedByUser { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? JiraLink { get; set; }
    }
}
