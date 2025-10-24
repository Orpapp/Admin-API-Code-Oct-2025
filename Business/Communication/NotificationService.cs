using Shared.Common;
using Shared.Resources;

namespace Business.Communication
{
    public class NotificationService : INotificationService
    {

        private readonly IEmailFunctions _emailFunctions;

        public NotificationService(IEmailFunctions emailFunctions)
        {
            _emailFunctions = emailFunctions;
        }

        public void EmailVerification(string toEmail, string name, string token, string subject)
        {
            var emailConfirmationLink = $"{SiteKeys.SiteUrl}Account/EmailVerification?token={System.Web.HttpUtility.UrlEncode(token)}";
            _emailFunctions.EmailVerification(toEmail, subject, name, emailConfirmationLink);
        }
        public void SendResetPasswordEmail(string emailsubject, string token, string toEmail, string? name)
        {
            var reseturl = $"{SiteKeys.SiteUrl}Account/ResetPassword?token={token}";
            _emailFunctions.SendResetPasswordEmail(ResourceString.ForgetPasswordSubject, reseturl, toEmail, name);
        }
        public void SendResetPasswordEmailToWebUser(string emailsubject, string token, string toEmail, string name)
        {
            var passwordResetLink = SiteKeys.SiteUrl + "Account/ResetPassword?Token=" + token;
            _emailFunctions.SendResetPasswordEmailToWebUser(ResourceString.ForgetPasswordSubject, passwordResetLink, toEmail, name);
        }
        public void SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query)
        {
            _emailFunctions.SendContactUsMailToAdmin(emailsubject, userName, userEmail, query);
        }
    }
}
