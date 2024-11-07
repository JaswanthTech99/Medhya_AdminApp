using System.Net;
using System.Text.Json;

namespace Medhya.API.Middleware;


public class ExceptionGlobalHandlingMiddleware 
{
    private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionGlobalHandlingMiddleware> _logger;
    public ExceptionGlobalHandlingMiddleware(RequestDelegate next, ILogger<ExceptionGlobalHandlingMiddleware> logger)
	{
        _next = next;
		_logger = logger;
    }

    
    public async Task Invoke(HttpContext context)
    {
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex, _logger) ;
		}
    }

	private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger _logger)
	{
		var code = HttpStatusCode.InternalServerError; //500 if unexpected
		var result = JsonSerializer.Serialize(new { error = "An error occured while processing your request", message = ex.Message.ToString() });
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)code;
        _logger.LogError(ex.ToString());
		return context.Response.WriteAsync(result);

	}
}
