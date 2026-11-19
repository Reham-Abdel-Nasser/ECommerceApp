using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenService refreshTokenService)
        : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var validateResult = await identityService.ValidateCredentialsAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // Generic failure for wrong password / missing user — never reveal which.
        // EmailNotConfirmed is intentional so the client can prompt for verification.
        if (validateResult.IsFailure)
        {
            return Result<AuthResponse>.Failure(
                validateResult.Error!.Code == IdentityErrors.EmailNotConfirmed.Code
                    ? IdentityErrors.EmailNotConfirmed
                    : IdentityErrors.InvalidCredentials);
        }

        var user = validateResult.Value!;
        var roles = await identityService.GetRolesAsync(user.UserId, cancellationToken);
        var accessToken = jwtTokenGenerator.GenerateToken(
            user.UserId,
            user.Email,
            user.DisplayName,
            roles);
        var refreshToken = await refreshTokenService.IssueAsync(user.UserId, cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            user.UserId,
            user.Email,
            user.DisplayName,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc));
    }
}