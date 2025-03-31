namespace Investigator.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IApplicationUserRepository ApplicationUser { get; }
        public ITemplateRepository Template { get; }
        public IQuestionRepository Question { get; }
        public IFormRepository Form { get; }
        public IResponseRepository Response { get; }
        public ICommentRepository Comment { get; }
        public ILikeRepository Like { get; }
        public ITemplateTagRepository TemplateTag { get; }
        public IQuestionOptionRepository QuestionOption { get; }
        public IJiraTicketRepository JiraTicket { get; }
        public ITemplateQuestionRepository TemplateQuestion { get; }
        void Save();
    }
}
