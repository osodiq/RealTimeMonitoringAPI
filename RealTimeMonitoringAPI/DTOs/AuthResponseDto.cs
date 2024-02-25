namespace RealTimeMonitoringAPI.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
