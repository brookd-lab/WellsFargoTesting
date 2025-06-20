using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public interface IJwtService
{
    Task<AuthenticationResult> GenerateTokenAsync(ApplicationUser user);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string refreshToken);
}

public class AuthenticationResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<JwtService> _logger;

    public JwtService(JwtSettings jwtSettings, ApplicationDbContext context, ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings;
        _context = context;
        _logger = logger;
    }

    public async Task<AuthenticationResult> GenerateTokenAsync(ApplicationUser user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);
            var expiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("firstName", user.FirstName ?? string.Empty),
                new("lastName", user.LastName ?? string.Empty),
                new("department", user.Department ?? string.Empty),
                new("isSsoUser", user.IsSsoUser.ToString()),
                new("ssoDomain", user.SsoDomain ?? string.Empty)
            };

            // Add role claims[1]
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryDate,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedDate = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiryDate,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
            return new AuthenticationResult
            {
                Success = false,
                Error = "Token generation failed"
            };
        }
    }

    //private async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    //{
    //    try
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

    //        var validationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(key),
    //            ValidateIssuer = true,
    //            ValidIssuer = _jwtSettings.Issuer,
    //            ValidateAudience = true,
    //            ValidAudience = _jwtSettings.Audience,
    //            ValidateLifetime = true,
    //            ClockSkew = TimeSpan.Zero
    //        };

    //        var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    //        return principal;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogWarning(ex, "Token validation failed");
    //        return null;
    //    }
    //}


    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return new AuthenticationResult
            {
                Success = false,
                Error = "Invalid or expired refresh token"
            };
        }

        var user = await _context.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            return new AuthenticationResult
            {
                Success = false,
                Error = "User not found"
            };
        }

        // Revoke old refresh token
        storedToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return await GenerateTokenAsync(user);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }

    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }



}
