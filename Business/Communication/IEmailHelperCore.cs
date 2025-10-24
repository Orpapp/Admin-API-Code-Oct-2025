namespace Business.Communication
{
    public interface IEmailHelperCore
    {
        public bool Send(string body, string recipient, string subject, string recipientCC, string recipientBCC);
        public string GenerateEmailTemplateFor(string templateName, params MessageKeyValue[] args);
        public string GenerateEmailTemplateWithfull(string filePath, params MessageKeyValue[] args);
        public bool IsValidEmailAddress(string emailAddress);
    }
}
