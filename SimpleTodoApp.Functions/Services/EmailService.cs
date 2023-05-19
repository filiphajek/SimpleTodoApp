using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace SimpleTodoApp.Functions.Services;

public class EmailService
{
    public async Task SendEmail(string email, string userName, string message)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Todo app", "simpletodoapp@seznam.cz"));
        mailMessage.To.Add(new MailboxAddress(userName, email));
        mailMessage.Subject = "Todo item deadline";
        mailMessage.Body = new TextPart("plain")
        {
            Text = message
        };

        var password = Environment.GetEnvironmentVariable("EmailPassword");

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync("smtp.seznam.cz", 587, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync("simpletodoapp@seznam.cz", password);
        await smtpClient.SendAsync(mailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}
