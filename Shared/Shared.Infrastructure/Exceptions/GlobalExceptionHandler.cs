using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Qwestr.Shared.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken ct)
    {
        if (exception is OperationCanceledException && ct.IsCancellationRequested)
        {
            return false;
        }

        var (statusCode, title) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
            ValidationException e => (StatusCodes.Status400BadRequest, e.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled server error");
        }
        else
        {
            _logger.LogWarning(exception, "Handled client error");
        }

        if (context.Response.HasStarted)
        {
            return false;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problem = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem, cancellationToken: ct);

        return true;
    }
}