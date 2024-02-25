using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealTimeMonitoringAPI.Model
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpDated { get; set; } = DateTime.Now;
        public bool? IsFlagged { get; set; } = false;
        public bool? IsEmailSent { get; set; } = false;
        public string? TransactionTier { get; set; } = "tier1";
        public bool? IsDeleted { get; set; } = false;
    }
    
}
