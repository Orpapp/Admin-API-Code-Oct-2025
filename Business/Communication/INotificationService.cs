namespace Business.Communication
{
    public interface INotificationService
    {
        public void EmailVerification(string toEmail, string name, string token, string subject);
        public void SendResetPasswordEmail(string emailsubject, string token, string toEmail, string? name);
        public void SendResetPasswordEmailToWebUser(string emailsubject, string token, string toEmail, string name);
        public void SendContactUsMailToAdmin(string emailsubject, string userName, string userEmail, string query);
    }
}
