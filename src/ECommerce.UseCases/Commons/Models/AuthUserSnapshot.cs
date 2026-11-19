namespace ECommerce.UseCases.Commons.Models;

public sealed record AuthUserSnapshot(Guid UserId, string Email, string? DisplayName);