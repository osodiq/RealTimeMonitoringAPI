namespace RealTimeMonitoringAPI.DTOs
{
    public class EmailResponse
    {
        public string Recipient { get; set; }
        public bool Status { get; set; }
        public string ResponseString { get; set; }
        public EmailResponse()
        {
            Status = false;
        }
    }
}
