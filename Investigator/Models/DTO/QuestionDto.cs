namespace Investigator.Models.DTO
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } 
        public string Type { get; set; }
        public int Order { get; set; }
        public bool IsOptional { get; set; }
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
