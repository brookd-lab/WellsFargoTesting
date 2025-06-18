using System.ComponentModel.DataAnnotations;

namespace ATMBankingAPI.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^\d{10,16}$", ErrorMessage = "Account number must be 10-16 digits")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits")]
        public string PIN { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AccountHolderName { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal Balance { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
