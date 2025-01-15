using Investigator.Models.DTOs;

namespace Investigator.Models.DTOs
{
    public class FormDto
    {
        public int FormId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        public string CreatorId { get; set; }
        public ICollection<QuestionDto> Questions { get; set; }
    }
}
