using Investigator.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models
{
    [Index(nameof(Title))]
    public class Template
    {
        [Key]
        public int TemplateId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Topic { get; set; } = string.Empty;
        public enum VisibilityType
        {
            Public,
            Restricted
        }
        [Required]
        public VisibilityType Visibility { get; set; } = VisibilityType.Public;
        public string? ImageId { get; set; }

        [Required]
        public string ? CreatorId { get; set; }

        [ForeignKey(nameof(CreatorId))]
        [ValidateNever]
        public ApplicationUser Creator { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public int Point { get; set; } = 0;
        public ICollection<TemplateTag> Tags { get; set; } = new List<TemplateTag>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
