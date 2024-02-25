namespace RealTimeMonitoringAPI.DTOs
{
    public class EmailRequest
    {
        public bool isBodyHtml { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public string ToEmail { get; set; }
        public string ToCc { get; set; }
        public EmailRequest()
        {

        }
    }
}
