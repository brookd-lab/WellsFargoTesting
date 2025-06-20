using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SsoSettings _ssoSettings;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        UserManager<ApplicationUser> userManager,
        SsoSettings ssoSettings,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _ssoSettings = ssoSettings;
        _logger = logger;
    }

    [HttpPost("sso-login")]
    public async Task<IActionResult> SsoLogin([FromBody] SsoLoginRequest request)
    {
        try
        {
            // Validate SSO token (this would typically validate against your SSO provider)
            var ssoUser = await ValidateSsoTokenAsync(request.SsoToken);
            if (ssoUser == null)
            {
                return Unauthorized(new { error = "Invalid SSO token" });
            }

            // Check if domain is allowed
            var emailDomain = ssoUser.Email.Split('@')[1];
            if (!_ssoSettings.AllowedDomains.Contains(emailDomain))
            {
                return Unauthorized(new { error = "Domain not authorized for SSO" });
            }

            // Find or create user
            var user = await _userManager.FindByEmailAsync(ssoUser.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = ssoUser.Email,
                    Email = ssoUser.Email,
                    FirstName = ssoUser.FirstName,
                    LastName = ssoUser.LastName,
                    Department = ssoUser.Department,
                    EmailConfirmed = true,
                    IsSsoUser = true,
                    SsoDomain = emailDomain,
                    Roles = ssoUser.Roles ?? new List<string> { "User" }
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new { error = "Failed to create user account" });
                }
            }
            else
            {
                // Update user information from SSO
                user.FirstName = ssoUser.FirstName;
                user.LastName = ssoUser.LastName;
                user.Department = ssoUser.Department;
                user.LastLogin = DateTime.UtcNow;
                user.Roles = ssoUser.Roles ?? user.Roles;

                await _userManager.UpdateAsync(user);
            }

            // Generate JWT tokens
            var authResult = await _jwtService.GenerateTokenAsync(user);
            if (!authResult.Success)
            {
                return StatusCode(500, new { error = authResult.Error });
            }

            _logger.LogInformation("SSO login successful for user {Email} from domain {Domain}",
                ssoUser.Email, emailDomain);

            return Ok(new
            {
                accessToken = authResult.AccessToken,
                refreshToken = authResult.RefreshToken,
                expiresAt = authResult.ExpiresAt,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    department = user.Department,
                    roles = user.Roles
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSO login failed for token {Token}", request.SsoToken);
            return StatusCode(500, new { error = "SSO login failed" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _jwtService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Success)
        {
            return Unauthorized(new { error = result.Error });
        }

        return Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            department = user.Department,
            roles = user.Roles,
            isSsoUser = user.IsSsoUser,
            ssoDomain = user.SsoDomain
        });
    }

    private async Task<SsoUserInfo?> ValidateSsoTokenAsync(string ssoToken)
    {
        // This is a simplified SSO token validation
        // In production, you would validate against your actual SSO provider
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(ssoToken);

            var email = jsonToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            var firstName = jsonToken.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
            var lastName = jsonToken.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;
            var department = jsonToken.Claims.FirstOrDefault(x => x.Type == "department")?.Value;
            var roles = jsonToken.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();

            if (string.IsNullOrEmpty(email))
                return null;

            return new SsoUserInfo
            {
                Email = email,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Department = department ?? string.Empty,
                Roles = roles.Any() ? roles : new List<string> { "User" }
            };
        }
        catch
        {
            return null;
        }
    }
}

public class SsoLoginRequest
{
    public string SsoToken { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class SsoUserInfo
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
