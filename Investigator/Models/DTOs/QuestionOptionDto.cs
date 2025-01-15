using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Investigator.Models.DTOs
{
    public class QuestionOptionDto
    {

        public int OptionId { get; set; }
        public string Text { get; set; }
        public int QuestionId { get; set; }
    }
}
