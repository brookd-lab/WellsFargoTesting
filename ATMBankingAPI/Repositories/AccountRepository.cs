using ATMBankingAPI.Data;
using ATMBankingAPI.Interfaces;
using ATMBankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ATMBankingAPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingContext _context;

        public AccountRepository(BankingContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.IsActive);
        }

        public async Task<bool> ValidateAccountAndPINAsync(string accountNumber, string pin)
        {
            var account = await GetAccountByNumberAsync(accountNumber);
            return account != null && account.PIN == pin;
        }

        public async Task<decimal> GetBalanceAsync(string accountNumber)
        {
            var account = await GetAccountByNumberAsync(accountNumber);
            return account?.Balance ?? 0;
        }

        public async Task<bool> AccountExistsAsync(string accountNumber)
        {
            return await _context.Accounts
                .AnyAsync(a => a.AccountNumber == accountNumber && a.IsActive);
        }
    }
}
