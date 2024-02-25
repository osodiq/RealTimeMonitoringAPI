using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealTimeMonitoringAPI.DTOs
{
    public class NewUserReq
    {
        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        [Required(ErrorMessage = "Please enter your email")]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
    }
}
