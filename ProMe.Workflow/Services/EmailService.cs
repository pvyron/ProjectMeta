using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ProMe.Abstractions;
using ProMe.Workflow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProMe.Workflow.Services;
internal class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly ushort _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _smtpEmailSender;

    public EmailService(IConfiguration configuration)
    {
        _smtpServer = configuration.GetValue<string>("EmailConfiguration:SmtpServer")!;
        _smtpPort = configuration.GetValue<ushort>("EmailConfiguration:Port")!;
        _smtpUsername = configuration.GetValue<string>("EmailConfiguration:Username")!;
        _smtpPassword = configuration.GetValue<string>("EmailConfiguration:Password")!;
        _smtpEmailSender = configuration.GetValue<string>("EmailConfiguration:From")!;
    }

    public async ValueTask<Guid> SendVerificationEmail(string email)
    {
        var verificationId = Guid.NewGuid();

        var verificationSoup = $"{Convert.ToBase64String(verificationId.ToByteArray())}.{Convert.ToBase64String(Encoding.ASCII.GetBytes(email))}";

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(MailboxAddress.Parse(_smtpEmailSender));
        emailMessage.To.Add(InternetAddress.Parse(email));
        emailMessage.Subject = "Contacts - Email verification";
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"Follow the <a href=\"https://localhost:7076/VerifyEmail/{verificationSoup}\">link</a> to verify your email." };

        using var mailClient = new SmtpClient();
        await mailClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.Auto);
        await mailClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
        await mailClient.SendAsync(emailMessage);
        await mailClient.DisconnectAsync(true);

        return verificationId;
    }
}
