namespace Business.Communication
{
    public class MessageKeyValue
    {
        /// <summary>
        /// html key in template
        /// </summary>
        public string? HtmlKey { get; set; }

        /// <summary>
        /// value on HtmlKey
        /// </summary>
        public string? HtmlValue { get; set; }

        /// <summary>
        /// constructor of value class
        /// </summary>
        /// <param name="key">Html key</param>
        /// <param name="value">Html value</param>
        public MessageKeyValue(string? key, string? value)
        {
            HtmlKey = key;
            HtmlValue = value;
        }
    }
}
