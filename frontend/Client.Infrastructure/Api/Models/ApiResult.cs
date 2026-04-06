using System.Net;

namespace Client.Infrastructure.Api.Models;

public record IdResponse(Guid Id);

public class ApiResult
{
    public bool IsSuccess { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public string? Error { get; init; }

    public static ApiResult Success() => new() { IsSuccess = true };
    public static ApiResult Failure(HttpStatusCode code, string error) =>
        new() { IsSuccess = false, StatusCode = code, Error = error };
}

public class ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public string? Error { get; init; }

    public static ApiResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static ApiResult<T> Failure(HttpStatusCode code, string error) =>
        new() { IsSuccess = false, StatusCode = code, Error = error };
}