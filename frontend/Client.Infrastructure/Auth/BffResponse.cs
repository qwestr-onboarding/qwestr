namespace Client.Infrastructure.Auth;

public class BffResponse<T>
{
    public bool IsSuccess { get; init; }
    public int Status { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
}

public class BffResponse
{
    public bool IsSuccess { get; init; }
    public int Status { get; init; }
    public string? Error { get; init; }
}