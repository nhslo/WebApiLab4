namespace WebApiLab4.Results;

public class ReturnResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Result { get; init; }
    public IReadOnlyList<string> ErrorMessage { get; init; } = Array.Empty<string>();

    public static ReturnResult<T> Success(T result) =>
        new() { IsSuccess = true, Result = result, ErrorMessage = Array.Empty<string>() };

    public static ReturnResult<T> Failure(IEnumerable<string> errors) =>
        new() { IsSuccess = false, Result = default, ErrorMessage = errors.ToArray() };

    public static ReturnResult<T> Failure(string error) => Failure(new[] { error });
}
