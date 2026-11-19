using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.Infrastructure.Payments;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Commons.Settings;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerce.API.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/payments")
            .WithTags("Payments")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0));

        // Public - Stripe calls this (no JWT)
        group.MapPost("/webhook", async (
            HttpRequest request,
            IPaymentService paymentService,
            IOptions<StripeSettings> stripeOptions,
            ILogger<StripePaymentService> logger,
            CancellationToken ct) =>
        {
            // json(body), header(Stripe-Signature) hmac (json + secret)
            var json = await new StreamReader(request.Body).ReadToEndAsync(ct);
            var signature = request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrWhiteSpace(signature))
                return Results.BadRequest("Missing Stripe-Signature header.");

            try
            {
                var StripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    stripeOptions.Value.WebhookSecret);

                switch (StripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:
                        {
                            var succeeded = StripeEvent.Data.Object as PaymentIntent;
                            if (succeeded is not null)
                                await paymentService.PaymentSucceeded(succeeded.Id, ct);
                            break;
                        }

                    case EventTypes.PaymentIntentPaymentFailed:
                        {
                            var failed = StripeEvent.Data.Object as PaymentIntent;
                            if (failed is not null)
                                await paymentService.PaymentFailed(failed.Id, ct);
                            break;
                        }

                    default:
                        break;
                }

                return Results.Ok();
            }
            catch(StripeException ex)
            {
                logger.LogWarning(
                    ex,
                    "Invalid Stripe webhook signature.");

                return Results.BadRequest(ex.Message);
            }

            
        })
        .AllowAnonymous()
        .WithSummary("Stripe webhook")
        .WithDescription("Verifies Stripe-Signature and marks orders paid on payment_intent.succeeded.");

        return endpoints;
    }
}
