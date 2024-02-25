using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealTimeMonitoringAPI.DTOs
{
    public class TransactionReq
    {
        [Required(ErrorMessage = "Please enter your email")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Please enter your amount")]
        public decimal Amount { get; set; }       
    }
}
