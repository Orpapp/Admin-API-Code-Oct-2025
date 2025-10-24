using Microsoft.Extensions.Options;
using Shared.Common;

namespace Business.Communication
{
    public class EmailFunctions : IEmailFunctions
    {
        private readonly EmailConfigurationKeys _configurationKey;
        private readonly string _emailPath;
        private readonly string _logoPath;
        private readonly IEmailHelperCore _emailHelper;
        public EmailFunctions(IOptions<EmailConfigurationKeys> configurationKey, IEmailHelperCore emailHelperCore)
        {
            _configurationKey = configurationKey.Value;
            _emailPath = string.Format("{0}{1}", SiteKeys.SitePhysicalPath, SiteKeys.EmailTempaltePath);
            _logoPath = string.Format("{0}/favicon.ico", SiteKeys.SiteUrl);
            _emailHelper = emailHelperCore;
        }

        /// <summary>
        /// By this method we can send email through deligate
        /// </summary>
        /// <param name="emailHelper"></param>
        public void SendEmailThroughDelegate(string body, string recipient, string subject, string recipientCC, string recipientBCC)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                _emailHelper.Send(body, recipient, subject, recipientCC, recipientBCC);
            });
        }
        public void EmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink)
        {
            var serverFilePath = string.Format("{0}{1}", _emailPath, "\\Registration.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                    new MessageKeyValue("##Name##", name),
                    new MessageKeyValue("##EmailVerificationLink##", emailVerificationLink),
                    new MessageKeyValue("##AppName##", SiteKeys.AppName)
                );
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            SendEmailThroughDelegate(body, recipient, subject, string.Empty, string.Empty);
        }
        public void SendResetPasswordEmail(string emailsubject, string resetUrl, string toEmail, string? name)
        {
            var serverFilePath = string.Format("{0}{1}", _emailPath, "\\ResetPassword.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                        new MessageKeyValue("##Name##", name),
                        new MessageKeyValue("##ResetUrl##", resetUrl),
                        new MessageKeyValue("##ImagePath##", _logoPath),
                        new MessageKeyValue("##AppName##", SiteKeys.AppName)
                        );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
        public void SendResetPasswordEmailToWebUser(string emailsubject, string resetUrl, string toEmail, string name)
        {
            var serverFilePath = string.Format("{0}{1}", _emailPath, "\\ResetPassword.html");

            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                new MessageKeyValue("##Name##", name),
                new MessageKeyValue("##ResetUrl##", resetUrl),
                new MessageKeyValue("##ImagePath##", _logoPath),
                new MessageKeyValue("##AppName##", SiteKeys.AppName)
                );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = toEmail.ToLower();
            string subject = emailsubject;
            SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
        public void SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query)
        {
            var serverFilePath = string.Format("{0}{1}", _emailPath, "\\ContactUs.html");
            string body = _emailHelper.GenerateEmailTemplateWithfull(serverFilePath,
                            new MessageKeyValue("##UserName##", userName),
                            new MessageKeyValue("##UserEmail##", userEmail),
                            new MessageKeyValue("##Query##", query),
                            new MessageKeyValue("##ImagePath##", _logoPath),
                            new MessageKeyValue("##AppName##", SiteKeys.AppName)
                            );
            string recipientBCC = _configurationKey.EmailBCC ?? string.Empty;
            string recipientCC = _configurationKey.EmailCC ?? string.Empty;
            string recipient = _configurationKey.AdminEmail ?? string.Empty;
            string subject = emailsubject;
            SendEmailThroughDelegate(body, recipient, subject, recipientCC, recipientBCC);
        }
    }
}
