namespace Application.Features.Common.Models;

public class ServiceResponseGenerator<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }

    public static ServiceResponseGenerator<T> Failure(string? message) => new()
    {
        Message = message,
        Result = default,
        IsSuccess = false
    };

    public static ServiceResponseGenerator<T> Success(string? message, T result) => new()
    {
        Message = message,
        Result = result,
        IsSuccess = true
    };
}
