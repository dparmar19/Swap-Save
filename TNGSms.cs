namespace TNG.Shared.Lib.Communications.Text
{

    public class TNGSms
    {
        public string Id { get; set; }

        public TNGSmsAddress To { get; set; }

        public TNGSmsAddress From { get; set; }

        public int OrderId { get; set; }

        public Boolean IsSent { get; set; }

        public string Content { get; set; }

        public Boolean IsFailed { get; set; }

        public string ErrorMessage { get; set; }

        public string ReferenceId { get; set; }

        public bool IsOtp { get; set; }

    



    }

}
