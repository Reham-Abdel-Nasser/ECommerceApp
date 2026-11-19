using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;

public sealed class UpdateDeliveryMethodCommandHandler(
    IRepository<DeliveryMethod> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDeliveryMethodCommand, Result>
{
    public async Task<Result> Handle(
        UpdateDeliveryMethodCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return Result.Failure(DeliveryMethodErrors.NotFound);

        var nameExists = await repository.AnyAsync(
                new DeliveryMethodByNameSpecification(request.Name, request.Id),
                cancellationToken);

        if(nameExists)
            return Result.Failure(DeliveryMethodErrors.NameAlreadyExists);

        var updateResult = entity.Update(
            request.Name,
            request.Price,
            request.EstimatedDeliveryTime,
            request.Description,
            request.IsAvailable,
            request.DisplayOrder);

        if (updateResult.IsFailure)
            return updateResult;

        repository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}