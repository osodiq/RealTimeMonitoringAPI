using RealTimeMonitoringAPI.DTOs;
using RealTimeMonitoringAPI.Model;

namespace RealTimeMonitoringAPI.Services
{
    public interface IRepository
    {
        Task<bool> EvaluatePoliciesAndSendNotifications(TransactionReq req);
        Task<List<Transaction>> GetAllTransactions();
        Task<Transaction> GetTransactionById(int transactionId);
        Task<bool> DeleteTransactionById(int transactionId);
        Task<NewUserResp> Register(NewUserReq req);
        Task<User> GetUserById(Guid userId);
        Task<AuthResponseDto?> ValidateSecret(Guid userId, string secret);
        Task<NewUserResp> UpdateSecretAsyc(Guid userId);
        Task<List<User>> GetAllUsers();
        Task<bool> UpdateUserFullName(Guid userId, string fullName);
        Task<bool> UpdateUserName(Guid userId, string userName);
        Task<bool> DeleteUserById(Guid userId);
    }
}
