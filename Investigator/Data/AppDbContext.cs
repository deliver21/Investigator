using Investigator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Investigator.Data
{
    public class AppDbContext:IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }    
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateQuestion> TemplateQuestions { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<JiraTicket> JiraTickets { get; set; }
        public DbSet<FormFiller> FormFillers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JiraTicket>().HasOne(j => j.CreatedByUser).WithMany()
                .HasForeignKey(f => f.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TemplateTag>()
                .HasKey(t => t.TagId);

            modelBuilder.Entity<Template>()
                .HasMany(t => t.Tags)
                .WithMany();

            modelBuilder.Entity<Template>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TemplateQuestion>()
               .HasOne(q => q.Template).WithMany()
               .HasForeignKey(q => q.TemplateId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
              .HasOne(q => q.Form)
              .WithMany(f => f.Questions) // Ensure Form has this property
              .HasForeignKey(q => q.FormId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Form>()
             .HasOne(f => f.Template)
             .WithMany()
             .HasForeignKey(f => f.TemplateId)
             .OnDelete(DeleteBehavior.SetNull); // Ensure TemplateId is nullable

            modelBuilder.Entity<Form>()
                .HasOne(f => f.Creator).WithMany()
                .HasForeignKey(f => f.CreatorId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Form).WithMany()
                .HasForeignKey(r => r.FormId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Question).WithMany()
                .HasForeignKey(r => r.QuestionId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Template).WithMany()
                .HasForeignKey(c => c.TemplateId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ApplicationUser).WithMany()
                .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Template).WithMany()
                .HasForeignKey(l => l.TemplateId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.ApplicationUser).WithMany()
                .HasForeignKey(l => l.LikerId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Response>()
               .HasOne(r => r.Form)
               .WithMany()
               .HasForeignKey(r => r.FormId)
               .OnDelete(DeleteBehavior.Cascade); // Keep this cascade

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Question)
                .WithMany()
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Change from Cascade to Restrict

            modelBuilder.Entity<QuestionOption>()
                .HasOne(q => q.Question).WithMany()
                .HasForeignKey(q => q.QuestionId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormFiller>()
                .HasOne(f => f.Form).WithMany()
                .HasForeignKey(f => f.FormId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
