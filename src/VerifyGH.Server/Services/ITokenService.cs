using VerifyGH.Server.Models;

namespace VerifyGH.Server.Services;

public interface ITokenService
{
    Task<(string Token, DateTime Expiration)> GenerateJwtTokenAsync(ApplicationUser user);
}
