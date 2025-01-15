namespace Investigator.Models.ViewModels
{
    public class TemplateDetailsVM
    {
        public Template Template { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
        public IEnumerable<Like> Likes { get; set; } = new List<Like>();
    }
}
