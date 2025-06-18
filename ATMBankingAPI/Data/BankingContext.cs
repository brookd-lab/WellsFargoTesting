using ATMBankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ATMBankingAPI.Data
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed some test data for your ATM testing
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    AccountNumber = "1234567890123456", // 16 digits - matches your Angular validation
                    PIN = "1234",
                    AccountHolderName = "John Doe",
                    Balance = 2847.63m,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Account
                {
                    Id = 2,
                    AccountNumber = "9876543210", // 10 digits - minimum valid length
                    PIN = "5678",
                    AccountHolderName = "Jane Smith",
                    Balance = 1250.00m,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            );
        }
    }
}
