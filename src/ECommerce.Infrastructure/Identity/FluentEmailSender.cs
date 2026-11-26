using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Identity;

/// <summary>
/// Dev stub: logs the email (including verification code) to the console.
/// Later: inject <c>IFluentEmailFactory</c> and call FluentEmail SMTP send here.
/// </summary>
public sealed class FluentEmailSender(
    IFluentEmailFactory emailFactory,
    ILogger<FluentEmailSender> logger) : IEmailSender
{
    public async Task<Result> SendAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = emailFactory
                .Create()
                .To(toEmail)
                .Subject(subject)
                .Body(body, isHtml: false);

            var response = await email.SendAsync(cancellationToken);

            if (!response.Successful)
            {
                logger.LogError(
                    "Failed to send email to {ToEmail}. Errors: {Errors}",
                    toEmail,
                    string.Join("; ", response.ErrorMessages));

                return Result.Failure(IdentityErrors.EmailSendFailed);
            }

            logger.LogInformation(
               "Email sent successfully to {ToEmail}",
               toEmail);

            return Result.Success();

        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Unexpected error while sending email to {ToEmail}",
                toEmail);

            return Result.Failure(IdentityErrors.EmailSendFailed);
        }
    }
}