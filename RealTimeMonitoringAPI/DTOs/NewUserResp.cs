namespace RealTimeMonitoringAPI.DTOs
{
    public class NewUserResp
    {
        public Guid UserId { get; set; }
        public string Secret { get; set; }
        public string Description { get; set; }
    }
}
