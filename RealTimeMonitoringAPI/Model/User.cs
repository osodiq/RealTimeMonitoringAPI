using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RealTimeMonitoringAPI.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Secret { get; set; }
        public bool? IsFlagged { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateUpDated { get; set; } = DateTime.Now;
    }
}
