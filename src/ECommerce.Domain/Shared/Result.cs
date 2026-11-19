namespace ECommerce.Domain.Shared;

public class Result
{
    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException(
                "Success Result cannot have an error.");

        if (!isSuccess && error is null)
            throw new InvalidOperationException(
                "Failure Result must have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException(
                "Failure Result must have a valid error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public Error? Error { get; }

    public bool IsFailure => !IsSuccess;


    public static Result Success()
        => new(true, Error.None);

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new (false, error);
    }
        //=> new(false, error);
   
}