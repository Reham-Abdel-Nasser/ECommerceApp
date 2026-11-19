namespace ECommerce.UseCases.Messaging.Implementation;

internal interface IRequestHandlerExecutor
{
    Type RequestType { get; }

    Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken);
}
