namespace Business.Communication
{
    public interface IEmailFunctions
    {
        public void EmailVerification(string toEmail, string emailsubject, string name, string emailVerificationLink);
        public void SendResetPasswordEmail(string emailsubject, string resetUrl, string toEmail, string? name);
        public void SendResetPasswordEmailToWebUser(string emailsubject, string resetUrl, string toEmail, string name);
        public void SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query);
    }
}
