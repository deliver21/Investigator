using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Investigator.Models.DTOs
{
    public class JiraTicketDto
    {
        [Key]
        public int TicketId { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Priority { get; set; }
        public string? TemplateTitle { get; set; }
        public string PageLink { get; set; }
    }
}
