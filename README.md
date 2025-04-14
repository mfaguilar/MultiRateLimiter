# MultiRateLimiter
**Support for Multiple PartitionedRateLimiter (.NET)**

I created an implementation to overcome the limitation (still in place as of April 2025) in the design of .NET RateLimiters, which prevents multiple policies from being configured for the same endpoint.

For a simple scenario, which is what the software I implemented it in required, it works very well; it will probably require more details for more complex scenarios.

I haven't uploaded a full example project to this repository yet, just the key files, but I think it's easy enough for everyone to understand and implement in their own project.
Feel free to contact me if you have any questions or an improvement to make.

<ins>Summary of the files:</ins>

**EnableMultiRateLimitingAttribute**: Attribute tu use in Controllers. 

**MultiRateLimitingMiddleware**: The middleware that intercepts all the Controller's actions with EnableMultiRateLimitingAttribute.

**MultiRateLimiterService**: Just a simple service to store the Rate Limiters in memory.

**MultiRateLimiterOptions and RateLimitConfigs**: Config for the MultiRateLimiter. (name of the policy and the function that must be executed to validate it).

**Program.cs**: the classic configuration of the service. 
<br/><br/>

So with this implementation you can do something like this:

```[HttpPost("resetPassword")]
[EnableMultiRateLimiting(RateLimiterPolicy.PasswordReset, RateLimiterPolicy.PasswordResetGlobal)]
public async Task<ActionResult<ApiResponse<AuthData>>> ResetPassword([FromBody] ResetPasswordDto model)
{
}
```
