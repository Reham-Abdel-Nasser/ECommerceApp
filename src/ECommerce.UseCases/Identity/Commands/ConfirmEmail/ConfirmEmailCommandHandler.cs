using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IIdentityService identityService,
    IEmailVerificationCodeStore verificationCodeStore)
        : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var code = request.Code.Trim();

        var userResult = await identityService.GetUserByEmailAsync(email, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        if (await identityService.IsEmailConfirmedAsync(email, cancellationToken))
            return Result.Failure(IdentityErrors.EmailAlreadyConfirmed);

        var isValid = await verificationCodeStore.ValidateAndConsumeAsync(
            email,
            code,
            cancellationToken);

        if (!isValid)
            return Result.Failure(IdentityErrors.InvalidVerificationCode);

        return await identityService.ConfirmEmailAsync(email, cancellationToken);
    }
}