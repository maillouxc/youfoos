using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using YouFoos.Api.Config;
using YouFoos.Api.Dtos;
using YouFoos.Api.Dtos.Account;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IEmailSender"/>.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IEmailSender.SendEmailAsync(string, string, string)"/>.
        /// </summary>
        public async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            try
            {
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SenderAddress, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                mail.To.Add(new MailAddress(toAddress));

                using (var smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    if (string.IsNullOrEmpty(_emailSettings.AccountPassword))
                    {
                        smtp.EnableSsl = false;
                    }
                    else
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_emailSettings.AccountUsername, _emailSettings.AccountPassword);
                        smtp.EnableSsl = true;
                    }

                    await smtp.SendMailAsync(mail);   
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error("Failed to send email: {@e}", e);
                throw;
            }
        }

        /// <summary>
        /// Concrete implementation of <see cref="IEmailSender.SendNewUserWelcomeEmail(CreateAccountDto)"/>.
        /// </summary>
        public async Task SendNewUserWelcomeEmail(CreateAccountDto newUser)
        {
            const string subject = "Your new YouFoos account is ready";

            string body = 
            $@"Hi there {newUser.FirstAndLastName},
            <br/><br/>
            A new YouFoos account with your email address has been created. You can now login with this account.
            <br/><br/>
            Thanks,
            <br/><br/>
            YouFoos System";

            await SendEmailAsync(newUser.EmailAddress, subject, body);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IEmailSender.SendPasswordResetEmail(PasswordResetCode, int)"/>.
        /// </summary>
        public async Task SendPasswordResetEmail(PasswordResetCode resetCode, int codeDurationMinutes)
        {
            const string subject = "YouFoos Password Reset Code";
            
            string body =
            $@"Hey there,
            <br/><br/>
            A password reset was requested for your YouFoos account. If this was you,
            your reset code is {resetCode.Code}. This code will expire after {codeDurationMinutes} minutes.
            <br/><br/>
            If this wasn't you, you can safely ignore this message.
            <br/><br/>
            Thanks,
            <br/><br/>
            YouFoos System";

            await SendEmailAsync(resetCode.UserEmail, subject, body);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IEmailSender.SendTournamentCreationEmail(CreateTournamentRequest, string)"/>.
        /// </summary>
        public async Task SendTournamentCreationEmail(CreateTournamentRequest tournamentInfo, string toEmail)
        {
            const string subject = "New YouFoos Tournament Started";

            string body =
            $@"Hey there,

            <br/><br/>
            A new YouFoos tournament has been started: {tournamentInfo.Name}.
            <br/><br/>
            Description:
            <br/><br/>
            {tournamentInfo.Description}
            <br/><br/>
            This is an open tournament, so anyone can register - if you're interested, sign up fast, 
            before it fills up - there are only {tournamentInfo.PlayerCount} available.
            <br/><br/>
            Good Luck!
            <br/><br/>
            YouFoos System";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
