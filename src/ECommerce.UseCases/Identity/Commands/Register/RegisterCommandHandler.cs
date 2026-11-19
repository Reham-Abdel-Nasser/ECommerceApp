using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Commons.Settings;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IEmailVerificationCodeStore verificationCodeStore,
    IEmailSender emailSender,
    IOptions<EmailVerificationSettings> settings)
        : IRequestHandler<RegisterCommand, Result<EmailSentResponse>>
{
    public const string RegisteredMessage =
        "Registration successful. A verification code was sent to your email. Confirm your email before logging in.";

    public const string UnconfirmedResendMessage =
        "This email is registered but not confirmed. A new verification code was sent to your email.";
    private readonly EmailVerificationSettings _settings = settings.Value;

    public async Task<Result<EmailSentResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var existing = await identityService.GetUserByEmailAsync(email, cancellationToken);
        if (existing.IsSuccess)
        {
            if (await identityService.IsEmailConfirmedAsync(email, cancellationToken))
                return Result<EmailSentResponse>.Failure(IdentityErrors.EmailAlreadyExists);

            return await SendVerificationAsync(
                email,
                verificationCodeResent: true,
                UnconfirmedResendMessage,
                cancellationToken);
        }

        var createResult = await identityService.CreateUserAsync(
            email,
            request.Password,
            request.DisplayName,
            cancellationToken);

        if (createResult.IsFailure)
            return Result<EmailSentResponse>.Failure(createResult.Error!);

        return await SendVerificationAsync(
            email,
            verificationCodeResent: false,
            RegisteredMessage,
            cancellationToken);
    }

    private async Task<Result<EmailSentResponse>> SendVerificationAsync(
        string email,
        bool verificationCodeResent,
        string message,
        CancellationToken cancellationToken)
    {
        var code = GenerateCode(_settings.CodeLength);

        await verificationCodeStore.SaveAsync(email, code, cancellationToken);

        var sendResult = await emailSender.SendAsync(
            email,
            "Confirm your ECommerce account",
            $"Your verification code is: {code}\n\nThis code expires in {_settings.ExpirationMinutes} minutes.\n\nYou cannot sign in until you confirm this email.",
            cancellationToken);

        if (sendResult.IsFailure)
            return Result<EmailSentResponse>.Failure(sendResult.Error!);

        return Result<EmailSentResponse>.Success(
            new EmailSentResponse(email, verificationCodeResent, message));
    }

    private static string GenerateCode(int length)
    {
        var max = (int)Math.Pow(10, length);
        return RandomNumberGenerator.GetInt32(0, max).ToString($"D{length}");
    }
}