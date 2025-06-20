public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}

public class SsoSettings
{
    public List<string> AllowedDomains { get; set; } = new();
    public bool RequireEmailVerification { get; set; }
}
