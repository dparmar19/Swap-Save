using System.Net.Mail;

namespace TNG.Shared.Lib.Communications.Email
{

    public class TNGEmail
    {
        public int Id { get; set; }

        public TNGEMailAddress To { get; set; }

        public TNGEMailAddress From { get; set; }

        public TNGEMailAddress BCC { get; set; }

        public TNGEMailAddress CC { get; set; }
        public string ReplyToId { get; set; }
        public string Subject { get; set; }

        public Boolean IsSent { get; set; }

        public Boolean IsHTML { get; set; }

        public Boolean IsFailed { get; set; }

        public string Content { get; set; }

        public int OrderId { get; set; }

        public string ErrorMessage { get; set; }
        public string StoreName { get; set; }
        public System.Net.Mail.Attachment Attachment { get; set; }


    }

}