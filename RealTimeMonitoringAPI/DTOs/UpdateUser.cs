using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealTimeMonitoringAPI.DTOs
{
    public class UpdateUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        
    }
}
