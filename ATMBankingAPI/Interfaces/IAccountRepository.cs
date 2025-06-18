using ATMBankingAPI.Models;

namespace ATMBankingAPI.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
        Task<bool> ValidateAccountAndPINAsync(string accountNumber, string pin);
        Task<decimal> GetBalanceAsync(string accountNumber);
        Task<bool> AccountExistsAsync(string accountNumber);
    }
}
