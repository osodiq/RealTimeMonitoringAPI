namespace RealTimeMonitoringAPI.DTOs
{
    public class AuthReq
    {
        public Guid UserId { get; set; }
        public string Secret { get; set; }
    }
}
