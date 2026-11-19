
using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.Domain.Constants;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Commands.CancelOrder;
using ECommerce.UseCases.Orders.Commands.CreateOrder;
using ECommerce.UseCases.Orders.Commands.CreateOrderPayment;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Queries.GetMyOrders;
using ECommerce.UseCases.Orders.Queries.GetOrderById;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/orders")
            .WithTags("Orders")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .RequireAuthorization(o => o.RequireRole(Roles.All))
            .AddEndpointFilter<AuditEndpointFilter>();

        // Create Order
        group.MapPost("/", async (
            [FromBody] CreateOrderCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            if (result.IsFailure)
                return result.Problem(httpContext);

            return Results.CreatedAtRoute(
                routeName: "GetOrderById",
                routeValues: new { id = result.Value!.Id },
                value: ApiResponse<OrderResponse>.Ok(
                    result.Value,
                    httpContext.TraceIdentifier,
                    "Order created successfully"));
        })
        .RequireRateLimiting("WritePolicy")
        .WithSummary("Create order (checkout)")
        .WithDescription(
            "Creates an order from the authenticated user's basket, then clears the basket.")
        .Produces<ApiResponse<OrderResponse>>(
            StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Get My Orders
        group.MapGet("/", async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetMyOrdersQuery(
                    pageNumber <= 0 ? 1 : pageNumber,
                    pageSize <= 0 ? 10 : pageSize),
                ct);

            return result.FromPagedResult(
                httpContext,
                pageNumber <= 0 ? 1 : pageNumber,
                pageSize <= 0 ? 10 : pageSize,
                ApiMessages.OrdersRetrieved);
        })
        .RequireRateLimiting("ReadPolicy")
        .WithSummary("Get current user orders")
        .Produces<ApiResponse<IReadOnlyList<OrderResponse>>>(
            StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // Get Order By Id
        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetOrderByIdQuery(id),
                ct);

            return result.FromResult(
                httpContext,
                ApiMessages.OrderRetrieved);
        })
        .WithName("GetOrderById") // Important: Required by CreatedAtRoute
        .WithSummary("Get order by id")
        .Produces<ApiResponse<OrderResponse>>(
            StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Cancel Order
        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new CancelOrderCommand(id),
                ct);

            return result.FromResult(
                httpContext,
                ApiMessages.OrderCancelled);
        })
        .WithSummary("Cancel pending order")
        .Produces<ApiResponse<OrderResponse>>(
            StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // Create Order Payment
        group.MapPost("/{id:guid}/pay", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new CreateOrderPaymentCommand(id),
                ct);

            return result.FromResult(
                httpContext,
                ApiMessages.PaymentIntentCreated);
        })
        .RequireRateLimiting("PaymentPolicy")
        .WithSummary(
            "Create Stripe PaymentIntent for order (or update amount if one already exists)")
        .WithDescription(
            "Uses order.Total (server-side). Calls CreatePaymentIntent when none exists; otherwise UpdatePaymentIntent. Returns clientSecret for Stripe.js.")
        .Produces<ApiResponse<PaymentClientSecretResponse>>(
            StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }
}
