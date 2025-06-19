using Investigator.Services.IServices;
using System.Net;
using System.Net.Mail;

namespace Investigator.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config) 
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mail = _config.GetSection("Email").GetValue<string>("Mail");
                var pw = "Matecode24012025";
                string code = _config.GetSection("Email").GetValue<string>("SecurityCode");
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, code)
                };

                return client.SendMailAsync(
                    new MailMessage(from: mail,
                    to: email,
                    subject, message
                    ));
            }
            catch(Exception ex )
            {
                Console.WriteLine(ex.Message);
                return Task.CompletedTask;
            }
            
        }
    }
}
