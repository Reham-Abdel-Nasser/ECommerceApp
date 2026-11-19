namespace ECommerce.API.Test;

public sealed record GenerateTestJwtRequest(
Guid UserId,
string Email,
string DisplayName,
string[] Roles);