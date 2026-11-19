using Microsoft.AspNetCore.Mvc;
using System.Threading.RateLimiting;

namespace ECommerce.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddAppRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            // Login: 5 requests per minute per IP
            options.AddPolicy("LoginPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(1)));

            // Register: 3 requests per hour per IP
            options.AddPolicy("RegisterPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 3,
                    window: TimeSpan.FromHours(1)));

            // Confirm Email: 5 requests per 5 minutes per IP
            options.AddPolicy("ConfirmEmailPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(5)));

            // Refresh Token: 10 requests per minute per IP
            options.AddPolicy("RefreshPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 10,
                    window: TimeSpan.FromMinutes(1)));

            // General API: 60 requests per minute per IP
            options.AddPolicy("GeneralPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 60,
                    window: TimeSpan.FromMinutes(1)));

            // Read Operations: 100 requests per sliding minute per IP
            options.AddPolicy("ReadPolicy", context =>
                CreateSlidingWindowLimiter(
                    context,
                    permitLimit: 100,
                    window: TimeSpan.FromMinutes(1),
                    segmentsPerWindow: 6));

            // Write Operations: 20 requests per minute per IP
            options.AddPolicy("WritePolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 20,
                    window: TimeSpan.FromMinutes(1)));

            // Payment: 5 requests per minute per IP
            options.AddPolicy("PaymentPolicy", context =>
                CreateFixedWindowLimiter(
                    context,
                    permitLimit: 5,
                    window: TimeSpan.FromMinutes(1)));

            // Heavy Operations: max 3 concurrent requests per IP
            //options.AddPolicy("HeavyOperationsPolicy", context =>
            //    RateLimitPartition.GetConcurrencyLimiter(
            //        partitionKey: GetClientIp(context),
            //        factory: _ => new ConcurrencyLimiterOptions
            //        {
            //            PermitLimit = 3,
            //            QueueLimit = 0
            //        }));

            // Standard 429 response
            options.OnRejected = async (context, cancellationToken) =>
            {
                var response = context.HttpContext.Response;

                response.StatusCode = StatusCodes.Status429TooManyRequests;
                response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Too Many Requests",
                    Detail = "You have exceeded the allowed request limit."
                };

                await response.WriteAsJsonAsync(
                    problemDetails,
                    options: null,
                    contentType: "application/problem+json",
                    cancellationToken: cancellationToken);
            };
        });

        return services;
    }

    private static FixedWindowRateLimiterOptions CreateFixedWindowOptions(
        int permitLimit,
        TimeSpan window)
    {
        return new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = window,
            QueueLimit = 0,
            AutoReplenishment = true
        };
    }

    private static RateLimitPartition<string> CreateFixedWindowLimiter(
        HttpContext context,
        int permitLimit,
        TimeSpan window)
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: GetClientIp(context),
            factory: _ => CreateFixedWindowOptions(
                permitLimit,
                window));
    }

    private static RateLimitPartition<string> CreateSlidingWindowLimiter(
        HttpContext context,
        int permitLimit,
        TimeSpan window,
        int segmentsPerWindow)
    {
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: GetClientIp(context),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                SegmentsPerWindow = segmentsPerWindow,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }

    private static string GetClientIp(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString()
               ?? "unknown";
    }
}
