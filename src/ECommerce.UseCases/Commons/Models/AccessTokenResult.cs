namespace ECommerce.UseCases.Commons.Models;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);