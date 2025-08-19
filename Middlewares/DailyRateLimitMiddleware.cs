namespace WeatherHistoryApi.Middlewares;

public class DailyRateLimitMiddleware(RequestDelegate next)
{
    private static int _requestCount = 0;
    private static DateTime _resetTime = DateTime.UtcNow.Date.AddDays(1); // próxima meia-noite UTC
    private const int LIMIT = 10000;

    public async Task InvokeAsync(HttpContext context)
    {
        if (DateTime.UtcNow >= _resetTime)
        {
            _requestCount = 0;
            _resetTime = DateTime.UtcNow.Date.AddDays(1);
        }

        if (_requestCount >= LIMIT)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Limite diário de 10.000 requisições atingido.");
            return;
        }

        _requestCount++;
        await next(context);
    }
}