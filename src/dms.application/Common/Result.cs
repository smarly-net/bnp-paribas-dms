namespace DMS.Application.Common;

public sealed class Result<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    public static Result<T> Ok(T data) => new() { Success = true, Data = data };
    public static Result<T> Fail(string error) => new() { Success = false, Error = error };
    public static Result<T> Fail(Exception ex) => new() { Success = false, Error = ex.Message };
}

public sealed class Result
{
    public bool Success { get; init; }
    public string? Error { get; init; }

    public static Result Ok() => new() { Success = true };
    public static Result Fail(string error) => new() { Success = false, Error = error };
    public static Result Fail(Exception ex) => new() { Success = false, Error = ex.Message };
}