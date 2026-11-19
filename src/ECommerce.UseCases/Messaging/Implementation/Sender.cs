using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Messaging.Implementation;

internal sealed class Sender(HandlerExecutorRegistry registry) : ISender
{
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var executor = registry.GetRequiredExecutor(request.GetType());
        var result = await executor.ExecuteAsync(request, cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException(
                $"Handler for '{request.GetType().Name}' returned null, but {typeof(TResponse).Name} was expected.");
        if (result is not TResponse typedResult)
        {
            throw new InvalidOperationException(
                $"Handler for '{request.GetType().Name}' returned {result.GetType().Name}, but {typeof(TResponse).Name} was expected.");
        }

        return typedResult;
    }
}
