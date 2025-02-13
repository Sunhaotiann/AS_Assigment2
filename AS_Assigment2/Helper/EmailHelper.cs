using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public static class EmailHelper
{
    public static async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("haotiansun241@gmail.com", "itrj dcdp pdzv xdcs"),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress("haotiansun241@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
