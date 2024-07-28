using SendGrid.Helpers.Mail;

namespace Datamarka_MVC.ConfigurationSections
{
    public class EmailOptions
    {
        public string? ApiKey { get; set; }
        public string? SenderEmail { get; set; }
        public string? SenderName { get; set; }

        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
