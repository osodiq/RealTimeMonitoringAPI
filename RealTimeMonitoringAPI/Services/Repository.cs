using Azure.Core;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RealTimeMonitoringAPI.Controllers;
using RealTimeMonitoringAPI.DTOs;
using RealTimeMonitoringAPI.Model;
using RealTimeMonitoringAPI.Shared;
using System.Net;
using System.Threading.Tasks;
using Transaction = RealTimeMonitoringAPI.Model.Transaction;

namespace RealTimeMonitoringAPI.Services
{
    public class Repository : IRepository
    {
        private readonly RealTimeMonitoringDbContext _context;
        private readonly IApiKeyService _security;
        private readonly IConfiguration config;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly ILogger<UserController> _log;
        public Repository(RealTimeMonitoringDbContext context, IApiKeyService security, IConfiguration config, IJwtTokenGenerator jwtTokenGenerator, IOptions<JwtSettings> jwtOptions, ILogger<UserController> log)
        {
            _context = context;
            _security = security;
            config = config;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtOptions = jwtOptions;
            _log = log;
        }

        #region Transaction
        /// <summary>
        /// Store the transaction in the database and trigger policy evaluation
        /// Evaluate policies and send notifications if condition met
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<bool> EvaluatePoliciesAndSendNotifications(TransactionReq req)
        {
            // check if userId exist on the user table
            var userId = _context.Users.Where(x => x.UserId == req.UserId && x.IsFlagged == false).FirstOrDefault();
            if(userId == null) { return false; }
            //save transaction request to the Db
            var saveTransaction = new Transaction()
            {
                UserId = req.UserId,
                Amount = req.Amount              
            };
            _context.Transactions.Add(saveTransaction);
             await _context.SaveChangesAsync();
            string body = "";
            string emailTo = "";
            string transactionTier = "";
            decimal threshold = 5000000;
            //get the transaction tier
            if(req.Amount > threshold)
            {
                transactionTier = "tier3";
            }
            if (req.Amount < threshold)
            {
                transactionTier = "tier1";
            }
            // Implement policy evaluation logic here
            _log.LogInformation("About to call implement policy evaluation........");
            if (IsTransactionAmountGreaterThanThreshold(req.Amount) && IsTransactionWithinOneMinute())
            {
                _log.LogInformation("AmountGreaterThanThreshold: About to call email service........");
                EmailService.EmailService.SendEmailNotification(body:$"TransactionID ==> {saveTransaction.Id}. <br> Transaction amount ==> {req.Amount} <br> TransactionTier ==> {transactionTier} <br> Transaction Date ==> {saveTransaction.DateCreated} <br>Description ==> Transaction amount exceeded {threshold} <br> Transaction occurred within 1 minute", config:config, type:2, emailTo: userId.UserName);
            }
            if (IsTransactionAmountLessThanThreshold(req.Amount) && IsTransactionWithinOneMinute())
            {
                _log.LogInformation("AmountLessThanThreshold: About to call email service........");
                EmailService.EmailService.SendEmailNotification(body: $"TransactionID ==> {saveTransaction.Id}. <br> Transaction amount ==> {req.Amount} <br> TransactionTier ==> {transactionTier} <br> Transaction Date ==> {saveTransaction.DateCreated} <br>Description ==> Transaction amount is less than {threshold} <br> Transaction occurred within 1 minute.", config:config, type:2, emailTo: userId.UserName);
            }
            
            if (IsTransactionAmountEqualToThreshold(req.Amount) && IsTransactionWithinOneMinute())
            {
                _log.LogInformation("AmountEqualToThreshold: About to call email service........");
                EmailService.EmailService.SendEmailNotification(body:$"TransactionID ==> {saveTransaction.Id}. <br> Transaction amount ==> {req.Amount} <br> TransactionTier ==> {transactionTier} <br> Transaction Date ==> {saveTransaction.DateCreated} <br>Description ==> Transaction amount is equal to {threshold} and transaction occurred within 1 minute.", config:config, type:2, emailTo: userId.UserName);
            }
            //update transaction status
            var updatestatus = _context.Transactions.Where(x => x.Id == saveTransaction.Id).FirstOrDefault();
            if (updatestatus != null)
            {
                updatestatus.TransactionTier = transactionTier;
                updatestatus.IsEmailSent = true;
                updatestatus.IsFlagged = true;
                _context.Transactions.Update(updatestatus);
                await _context.SaveChangesAsync();
            }
            _log.LogInformation("Policies evaluated and notifications sent.");
            return true;           

        }

        private static bool IsTransactionAmountGreaterThanThreshold(decimal amount)
        {
            // Implement logic to check if the transaction amount is greater than 5,000,000.
            decimal Threshold = 5000000;
            if (amount > Threshold)
            {
                return true;
            }
            return false; 
        }

        private static bool IsTransactionAmountLessThanThreshold(decimal amount)
        {
            // Implement logic to check if the transaction amount is less than 5,000,000.
            decimal Threshold = 5000000;
            if (amount < Threshold)
            {
                return true;
            }
            return false;
        }
        private static bool IsTransactionAmountEqualToThreshold(decimal amount)
        {
            // Implement logic to check if the transaction amount is equal to 5,000,000.
            decimal Threshold = 5000000;
            if (amount == Threshold)
            {
                return true;
            }
            return false;
        }

        private static bool IsTransactionWithinOneMinute()
        {
            // Implement logic to check if the transaction occurred within 1 minute.
            return true; 
        }
        
        public async Task<List<Transaction>> GetAllTransactions() 
        {
            // call DB to get the list of transactions.
             var transactions =  _context.Transactions.Where(x => x.IsDeleted == false).ToList();
            return transactions; 
        }
        /// <summary>
        /// Get transaction by transactionId
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<Transaction> GetTransactionById(int transactionId)
        {
            // call DB to get transaction by transactionId.
            var transactions = _context.Transactions.Where(x => x.Id == transactionId && x.IsDeleted == false).FirstOrDefault();          
            return transactions;
        }

        /// <summary>
        /// Delete transaction by TransactionById and update the Db
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTransactionById(int transactionId)
        {
            // call DB to get transaction to be deleted by transactionId.
            var transaction = _context.Transactions.Where(x => x.Id == transactionId && x.IsDeleted == false).FirstOrDefault();
            if (transaction != null)
            {
                transaction.IsDeleted = true;
                _context.Transactions.Update(transaction);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion

        #region user
        /// <summary>
        /// Register new user and generate userId
        /// send email notification for a new user
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<NewUserResp> Register(NewUserReq req)
        {
            //check if user exist
            var getUser = _context.Users.Where(x => x.UserName == req.UserName && x.IsFlagged == false).FirstOrDefault();
            if (getUser != null)
            {
                return new NewUserResp
                {
                  Secret = Decrypt(getUser.Secret),
                  UserId = getUser.UserId,
                  Description = "You have registered before"
                };
            }
            //save transaction request to the Db
            var apiSecret = _security.GenerateApiKey();
            var newUser = new User
            {
                UserName = req.UserName,
                FullName = req.FullName,                
                Secret = Encrypt(apiSecret)                
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            string emailTo = req.UserName;
            //call email service 
            EmailService.EmailService.SendEmailNotification(body: $"Your registration was successful. <br> UserID ==> {newUser.UserId} <br> API Secret ==> {apiSecret}", config: config, type:0, emailTo:req.UserName );
            return new NewUserResp { Secret = apiSecret , UserId = newUser.UserId, Description = "Registration was successful" };

        }
        public async Task<User> GetUserById(Guid userId)
        {
            // call DB to get user by userId of transactions.
            var user = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            return user;
        }
        /// <summary>
        /// Get all registered user 
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            // call DB to get the list of users.
            var users = _context.Users.Where(x => x.IsFlagged == false).ToList();
            return users;
        }
        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string Encrypt(string data)
        {
            return BCrypt.Net.BCrypt.HashPassword(data);
        }
        /// <summary>
        /// decrypt data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string Decrypt(string data)
        {
            return BCrypt.Net.BCrypt.HashPassword(data);
        }
        /// <summary>
        /// Service to Validate Secret
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public async Task<AuthResponseDto?> ValidateSecret(Guid userId, string secret)
        {
            string WebsiteUrl = "www.herconomy.com";
            var user = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(secret, user.Secret))
                {
                    var accessToken = _jwtTokenGenerator.GenerateToken(userId, WebsiteUrl);
                    const string tokenType = "Bearer";
                    return new AuthResponseDto
                    {
                        AccessToken = accessToken,
                        TokenType = tokenType,
                        ExpiresIn = _jwtOptions.Value.ExpiryMinutes
                    };
                }
            }
            return null;
        }
       
        /// <summary>
        /// Service to Update SecretAsyc 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>        
        public async Task<NewUserResp> UpdateSecretAsyc(Guid userId)
        {
            var user = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            if (user != null)
            {
                var apiKey = _security.GenerateApiKey();
                user.Secret = Encrypt(apiKey);
                user.DateUpDated = DateTime.Now;
                await _context.SaveChangesAsync();

                string emailTo = user.UserName;
                EmailService.EmailService.SendEmailNotification(body: $"Your reset secret was successful. <br> New API Secret ==> {apiKey}", config: config, type:1, emailTo: emailTo);
                return new NewUserResp { Secret = apiKey, UserId = userId};               
            }
            return null;
        }
        /// <summary>
        /// update Update User FullName
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserFullName(Guid userId, string fullName) 
        {
            //get user by UserId and update user full name
            var updateUser = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            if (updateUser != null)
            {
                updateUser.FullName = fullName;
                updateUser.DateUpDated = DateTime.Now;
                _context.Users.Update(updateUser);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        /// <summary>
        /// update Update UserName
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserName(Guid userId, string userName)
        {
            //get user by UserId and update userName
            var updateUser = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            if (updateUser != null)
            {
                updateUser.UserName = userName;
                updateUser.DateUpDated = DateTime.Now;
                _context.Users.Update(updateUser);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Delete user by userId and update the Db
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserById(Guid userId)
        {
            // call DB to get user to be deleted by userId.
            var user = _context.Users.Where(x => x.UserId == userId && x.IsFlagged == false).FirstOrDefault();
            if (user != null)
            {
                user.IsFlagged = true;               
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion

    }
}
