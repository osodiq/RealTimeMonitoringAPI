using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using RealTimeMonitoringAPI.DTOs;
using RealTimeMonitoringAPI.Services;

namespace RealTimeMonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IRepository _repo;
        private readonly ILogger<UserController> _log;
        public UserController(IRepository repo, ILogger<UserController> log)
        {
            _repo = repo;
            _log = log;
        }
        /// <summary>
        /// Endpoint to register new user, generate userId and save persist the record into the Db 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] NewUserReq req) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var res = await _repo.Register(req);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _log.LogError($" New user registration error ==>  {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Endpoint to get all created users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var req = await _repo.GetAllUsers();
                if (req == null)
                {
                    return NotFound();
                }
                return Ok(req);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Endpoint to get existing user by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("userId")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var req = await _repo.GetUserById(userId);
                if(req == null)
                {
                    return NotFound();
                }
                return Ok(req);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Endpoint to update userName by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPut("userId/userName")]
        public async Task<IActionResult> UpdateUserName(Guid userId, string userName)
        {
            try
            {
                var update  = await _repo.UpdateUserName(userId, userName);
                if(update == false)
                {
                  return NotFound();
                }
                return Ok(update);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to update fullName by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        [HttpPut("userId/fullName")]
        public async Task<IActionResult> UpdateUserFullName(Guid userId, string fullName)
        {
            try
            {
                var update = await _repo.UpdateUserFullName(userId, fullName);
                if(update == false)
                {
                    return NotFound() ;
                }
                return Ok(update);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Endpoint to delete user by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("userId")]
        public async Task<IActionResult> DeleteUserById(Guid userId)
        {
            try
            {
                var req = await _repo.DeleteUserById(userId);
                if(req == false) { return NotFound(); }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[HttpPost("auth")]
        //public async Task<IActionResult> Auth([FromBody] AuthReq req) 
        //{
        //    try
        //    {
        //        var tokenDetails = _repo.ValidateSecret(req.UserId, req.Secret);
        //        if (tokenDetails == null)
        //        {
        //            return NotFound("Invalid userId or secret");
        //        }
        //        return Ok(tokenDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPut("resetsecret")]

        //public async Task<IActionResult> ResetSecret(Guid userId)
        //{
        //    try
        //    {
        //        var tokenDetails = _repo.UpdateSecretAsyc(userId);
        //        if (tokenDetails == null)
        //        {
        //            return NotFound("Invalid userId or secret");
        //        }
        //        return Ok(tokenDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
