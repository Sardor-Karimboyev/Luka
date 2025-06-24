using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Luka.Exceptions;

internal sealed class ErrorHandlerMiddleware : IMiddleware
{
    private readonly IExceptionToResponseMapper _exceptionToResponseMapper;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(IExceptionToResponseMapper exceptionToResponseMapper, ILogger<ErrorHandlerMiddleware> logger)
    {
        _exceptionToResponseMapper = exceptionToResponseMapper;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var exceptionResponse = _exceptionToResponseMapper.Map(exception);
        context.Response.StatusCode = (int) (exceptionResponse?.StatusCode ?? HttpStatusCode.BadRequest);
        var response = exceptionResponse?.Response;
        if (response is null)
        {
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        context.Response.ContentType = "application/json";
        
        await context.Response.WriteAsJsonAsync(response);
    }
}
