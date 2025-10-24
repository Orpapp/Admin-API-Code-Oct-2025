using Microsoft.Extensions.Options;
using Shared.Common;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Business.Communication
{
    public class EmailHelperCore : IEmailHelperCore
    {
        private readonly EmailConfigurationKeys _configurationKey;

        /// <summary>
        ///  Constructor of email helper take value from config
        ///  Keys = MailServer, Port, MailAuthUser, MailAuthPass, EnableSSL, EmailFromAddress
        /// </summary>
        public EmailHelperCore(IOptions<EmailConfigurationKeys> configurationKey)
        {
            _configurationKey = configurationKey.Value;
        }

        /// <summary>
        /// send mail after all object send
        /// </summary>
        public bool Send(string body, string recipient, string subject, string recipientCC, string recipientBCC)
        {
            try
            {
                var message = new MailMessage()
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = body,
                    From = new MailAddress(string.IsNullOrEmpty(_configurationKey.EmailFromAddress) ? "" : _configurationKey.EmailFromAddress, _configurationKey.EmailFromName ?? _configurationKey.EmailFromAddress)
                };
                recipient = string.IsNullOrEmpty(recipient) ? "" : recipient;
                string[] arrRecipent = recipient.Split(',');
                foreach (var recipent in arrRecipent)
                {
                    string[] arrRecipentFromSimiColon = recipent.Split(';');
                    foreach (var recipentSC in arrRecipentFromSimiColon)
                    {
                        message.To.Add(new MailAddress(recipentSC));
                    }
                }


                if (!string.IsNullOrEmpty(recipientBCC))
                {
                    string[] arrRecipientBCC = recipientBCC.Split(';');
                    foreach (var itemRecipientBCC in arrRecipientBCC)
                    {
                        message.Bcc.Add(new MailAddress(itemRecipientBCC));
                    }
                }

                if (!string.IsNullOrEmpty(recipientCC))
                {
                    string[] arrRecipientCC = recipientCC.Split(';');
                    foreach (var itemRecipientCC in arrRecipientCC)
                    {
                        message.CC.Add(new MailAddress(itemRecipientCC));
                    }
                }

                var smtp = new SmtpClient();

             

                if (_configurationKey.MailAuthUser?.Length > 0 && _configurationKey.MailAuthPass?.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = string.IsNullOrEmpty(_configurationKey.MailServer) ? "" : _configurationKey.MailServer;
                    smtp.Port = _configurationKey.Port;
                    smtp.Credentials = new NetworkCredential(_configurationKey.MailAuthUser, _configurationKey.MailAuthPass);
                    smtp.EnableSsl = _configurationKey.EnableSSL;
                }
                smtp.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return ex != null && false;
            }
        }

        /// <summary>
        /// generate email body from template, get template folder from config "MailTemplateFolder"
        /// </summary>
        /// <param name="templateName">template name</param>
        /// <param name="args">MessageKeyValue arguments to set value in template</param>
        /// <returns>Html email body from template</returns>
        public string GenerateEmailTemplateFor(string templateName, params MessageKeyValue[] args)
        {
            var mailFolderTemplatePath = SiteKeys.SitePhysicalPath + SiteKeys.EmailTempaltePath + "/";
            var filePath = Path.Combine(mailFolderTemplatePath, templateName);
            return GenerateEmailTemplateWithfull(filePath, args);
        }

        /// <summary>
        /// generate email body from template
        /// </summary>
        /// <param name="filePath">Template full path</param>
        /// <param name="args">MessageKeyValue arguments to set value in template</param>
        /// <returns>Html email body from template</returns>
        public string GenerateEmailTemplateWithfull(string filePath, params MessageKeyValue[] args)
        {
            if (File.Exists(filePath))
            {
                var htmlString = File.ReadAllText(filePath);

                return args == null ? htmlString : args.Aggregate(htmlString, (current, item) => current.Replace(item.HtmlKey == null ? "" : item.HtmlKey, item.HtmlValue));
            }
            else
            {
                throw new System.ArgumentException("Template file Not found");
            }
        }

        /// <summary>
        /// Determines whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the email address is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (String.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }
    }
}
