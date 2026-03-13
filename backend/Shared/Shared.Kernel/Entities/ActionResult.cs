namespace Qwestr.Shared.Kernel.Entities;

public class ActionResult
{
    public bool IsSuccess { get; }
    
    public string? Error { get; }
    
    public bool IsFailure => !IsSuccess;

    protected ActionResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static ActionResult Success() => new(true, null);
    
    public static ActionResult Failure(string error) => new(false, error);
}

public class ActionResult<T> : ActionResult
{
    public T? Value { get; }

    private ActionResult(T value) : base(true, null) => Value = value;
    private ActionResult(string error) : base(false, error) { }

    public static ActionResult<T> Success(T value) => new(value);
    
    public new static ActionResult<T> Failure(string error) => new(error);
}