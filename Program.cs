... (rest of the code)

Func<OnRejectedContext, CancellationToken, ValueTask> onRejected = async (context, token) =>
{
    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

    logger.LogInformation(
        Resources.Exceptions.RateLimitExceeded,
        context.HttpContext.Connection.RemoteIpAddress,
        context.HttpContext.Request.Path);

    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
    await context.HttpContext.Response.WriteAsJsonAsync(new
    {
        Success = false,
        Message = Resources.Exceptions.TooManyRequests
    }, token);
};

var rtConfig = new RateLimitConfigs();

builder.Services.AddMultipleRateLimiter(options =>
{
    options.AddPolicy(RateLimiterPolicy.PasswordReset, rtConfig.GetConfigPolicyFunc(settings.PasswordReset))
    
    .AddPolicy(RateLimiterPolicy.PasswordResetGlobal, rtConfig.GetConfigPolicyFunc(settings.PasswordResetGlobal))

    .RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = onRejected;
});

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(RateLimiterPolicy.Register, rtConfig.GetConfigPolicyFunc(settings.Register))

    .AddPolicy(RateLimiterPolicy.Login, rtConfig.GetConfigPolicyFunc(settings.Login))

    .RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = onRejected;
});

... (rest of the code)