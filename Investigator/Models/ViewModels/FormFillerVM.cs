namespace Investigator.Models.ViewModels
{
    public class FormFillerVM
    {
        public IEnumerable<Question> Questions { get; set; } 
        public IEnumerable<FormFiller> FormFillers { get; set; }
        public Form Form { get; set; }
    }
}
