using ECommerce.UseCases.Commons.Models;

namespace ECommerce.UseCases.Commons.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult GenerateToken(Guid userId, string email, string? displayName, 
        IEnumerable<string> roles);
}
