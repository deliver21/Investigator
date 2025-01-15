using Investigator.Data;
using Investigator.Repository.IRepository;

namespace Investigator.Repository
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ITemplateRepository Template { get; private set; }
        public IQuestionRepository Question { get; private set; }
        public IFormRepository Form { get; private set; }
        public IResponseRepository Response { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public ILikeRepository Like { get; private set; }
        public ITemplateTagRepository TemplateTag { get; private set; }
        public IQuestionOptionRepository QuestionOption { get; private set; }
        public IJiraTicketRepository JiraTicket { get; private set; }
        public UnitOfWork(AppDbContext context) 
        {    
            _context = context;
            ApplicationUser = new ApplicationUserRepository(_context);
            Template = new TemplateRepository(_context);
            Question = new QuestionRepository(_context);
            Form = new FormRepository(_context);
            Response = new ResponseRepository(_context);
            Comment = new CommentRepository(_context);
            Like = new LikeRepository(_context);
            TemplateTag = new TemplateTagRepository(_context);
            QuestionOption = new QuestionOptionRepository(_context);
            JiraTicket = new JiraTicketRepository(_context);
        }
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
            
        }
    }
}
