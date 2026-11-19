namespace ECommerce.UseCases.Commons.Models;

public sealed record RefreshTokenIssueResult(
    Guid UserId,
    string Token,
    DateTimeOffset ExpiresAtUtc);