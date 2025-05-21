using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investigator.Models.DTOs
{
    public class FormSubmissionDto
    {
        public int FormId { get; set; }
        public string? Filler { get; set; }
        public string Answers { get; set; } = ""; // receive raw JSON string
        public List<IFormFile> Files { get; set; } = new(); 

        [NotMapped]
        public List<QuestionAnswerDto> ParsedAnswers =>
        JsonConvert.DeserializeObject<List<QuestionAnswerDto>>(Answers) ?? new();
    }
}
