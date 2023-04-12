using System.Security.Claims;

namespace Product.Api.Middleware
{
    public class UserScopeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserScopeMiddleware> _logger;

        public UserScopeMiddleware(RequestDelegate next, ILogger<UserScopeMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            _logger.LogDbg($"Instantiated {nameof(UserScopeMiddleware)} class.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Cheating - In real life we will get this value from JWT.
            ClaimsIdentity myIdentity = new(new List<Claim>{
                new Claim("AccountId", "123456", ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, "Mohammed", ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, "mhoque@inspirato.com", ClaimValueTypes.String),
            }, "Customer");

            context.User = new ClaimsPrincipal(myIdentity);

            if (context.User.Identity is { IsAuthenticated: true })
            {
                ClaimsPrincipal user = context.User;
                Claim? userEmailClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

                // Scope vs Correlation Id: Deterministic vs Non-Deterministic
                using (_logger.BeginScope($"User: {user.Identity.Name} ({userEmailClaim?.Value})"))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
