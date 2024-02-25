using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Transactions;

namespace RealTimeMonitoringAPI.DTOs
{
    
    public class GetTransaction
    {
        public HttpStatusCode ResponseCode { get; set; }
        public Transaction ResponseDetails { get; set; }

    }
}
