using Investigator.Models.DTOs;

namespace Investigator.Models.DTOs
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } 
        public string Type { get; set; }
        public int Order { get; set; }
        public bool IsOptional { get; set; }
        public int FormId { get; set; }
        public ICollection<QuestionOptionDto> Options { get; set; } = new List<QuestionOptionDto>();
    }
}
