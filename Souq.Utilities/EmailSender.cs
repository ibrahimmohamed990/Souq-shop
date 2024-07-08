using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Souq.Utilities
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("ibrahimm0067@gmail.com", "uwigbbhvirlavzkh");
            var mailMessage = new MailMessage
            {
                From = new MailAddress("ibrahimm0067@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true, 
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);


            //await client.SendMailAsync("ibrahimm0067@gmail.com", email, subject, htmlMessage);
            //return Task.CompletedTask;
        }
    }
}
