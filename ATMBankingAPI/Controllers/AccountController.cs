using ATMBankingAPI.Interfaces;
using ATMBankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ATMBankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("validate-account")]
        public async Task<IActionResult> ValidateAccount([FromBody] AccountValidationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool exists = await _accountRepository.AccountExistsAsync(request.AccountNumber);

            return Ok(new { IsValid = exists, Message = exists ? "Account found" : "Account not found" });
        }

        [HttpPost("validate-pin")]
        public async Task<IActionResult> ValidatePIN([FromBody] PINValidationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isValid = await _accountRepository.ValidateAccountAndPINAsync(request.AccountNumber, request.PIN);

            return Ok(new { IsValid = isValid, Message = isValid ? "PIN correct" : "Invalid PIN" });
        }

        [HttpGet("balance/{accountNumber}")]
        public async Task<IActionResult> GetBalance(string accountNumber)
        {
            // Validate account number format
            if (!System.Text.RegularExpressions.Regex.IsMatch(accountNumber, @"^\d{10,16}$"))
            {
                return BadRequest("Invalid account number format");
            }

            decimal balance = await _accountRepository.GetBalanceAsync(accountNumber);

            return Ok(new { AccountNumber = accountNumber, Balance = balance });
        }
    }

    // Simple request models with validation
    public class AccountValidationRequest
    {
        [Required]
        [RegularExpression(@"^\d{10,16}$", ErrorMessage = "Account number must be 10-16 digits")]
        public string AccountNumber { get; set; } = string.Empty;
    }

    public class PINValidationRequest
    {
        [Required]
        [RegularExpression(@"^\d{10,16}$", ErrorMessage = "Account number must be 10-16 digits")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be exactly 4 digits")]
        public string PIN { get; set; } = string.Empty;
    }
}
