using Microsoft.AspNetCore.Mvc;
using RealTimeMonitoringAPI.DTOs;
using RealTimeMonitoringAPI.Services;

namespace RealTimeMonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly IRepository _repo;
        public TransactionController(IRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// Endpoint for submitting transactions, evaluate policies and send email notifications when condition met.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        [HttpPost("transaction")]
        public async Task<IActionResult> Transaction([FromBody] TransactionReq transaction)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var req = await _repo.EvaluatePoliciesAndSendNotifications(transaction);
                if(req == false) { return BadRequest($" UserId des not exist"); }
                return Ok(req);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to get all transactions
        /// </summary>
        /// <returns></returns>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetallTransactions()
        {
            try
            {
                var req = await _repo.GetAllTransactions();
                if(req == null) { return NotFound(); }
                return Ok(req);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Endpoint to get transaction by transactionid
        /// </summary>
        /// <param name="transcationId"></param>
        /// <returns></returns>
        [HttpGet("transactionId")]
        public async Task<IActionResult> GetTransactionbyId(int transcationId)
        {
            try
            {
                var req = await _repo.GetTransactionById(transcationId);
                if(req == null) { return NotFound(); }
                return Ok(req);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to delete transaction by transactionId
        /// </summary>
        /// <param name="transcationId"></param>
        /// <returns></returns>
        [HttpDelete("transactionId")]
        public async Task<IActionResult> DeleteTransactionById(int transcationId)
        {
            try
            {
                var req = await _repo.DeleteTransactionById(transcationId);
                if (req == false) { return NotFound(); }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
