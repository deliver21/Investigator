namespace Investigator.Models.ViewModels
{
    public class FillerVM
    {
        public IEnumerable <Question> Questions { get; set; }
        public Form Form { get; set; }
        public FormFiller FormFiller { get; set; }
    }
}
