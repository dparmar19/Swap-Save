using TNG.Shared.Lib.Intefaces;
using MimeKit;
using System.Net.Mail;
using System.Net.Mime;
using TNG.Shared.Lib.Settings;
using System.Collections.Specialized;

namespace TNG.Shared.Lib.Communications.Email
{
    /// <summary>
    /// To send mail
    /// </summary>
    public class Emailer : IEmailer
    {
        private IAuthenticationService _authService;
        private EMailSettings eMailSettings;

        /// <summary>
        /// Constructor with configuration object
        /// </summary>EMailSettings eMailSettings
        /// <param name="emailSettings"></param>
        public Emailer(IAuthenticationService authService)
        {
            this._authService = authService;
        }

        public Emailer(EMailSettings eMailSettings)
        {
            this.eMailSettings = eMailSettings;
        }

        public bool SendMail(TNGEmail email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.eMailSettings.UserName, this.eMailSettings.UserName));
            message.To.Add(new MailboxAddress(email.To.Name, email.To.EmailId));
            message.Subject = email.Subject;

            message.Body = new MimeKit.TextPart("html")
            {
                Text = email.Content
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(this.eMailSettings.HostName, this.eMailSettings.Port, this.eMailSettings.UseSs1);
                if (!String.IsNullOrEmpty(this.eMailSettings.Item))
                {
                    client.AuthenticationMechanisms.Remove(this.eMailSettings.Item);
                }

                client.Authenticate(this.eMailSettings.UserName, this.eMailSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
        }

        [Obsolete]
        public bool SendMailSendGrid(TNGEmail email)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(new MailAddress(email.To.EmailId, email.To.Name));
            mailMsg.From = new MailAddress(this.eMailSettings.UserName, email.StoreName);
            mailMsg.Subject = email.Subject;
            if (email.Attachment != null)
            {
                mailMsg.Attachments.Add(email.Attachment);
            }
            mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.Content, null, MediaTypeNames.Text.Html));
            using (System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(this.eMailSettings.HostName, Convert.ToInt32(this.eMailSettings.Port)))
            {
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(this.eMailSettings.ApiKey, this.eMailSettings.Password);
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMsg);
            };
            return true;
        }
    }
}
