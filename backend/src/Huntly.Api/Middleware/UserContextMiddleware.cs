using System.Security.Claims;
using Huntly.Application.Shared.Interfaces;
using Huntly.Infra.Security;

namespace Huntly.Api.Middleware;

public class UserContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserContext userContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var concreteContext = (UserContext)userContext;

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                concreteContext.UserId = userId;
                concreteContext.IsAuthenticated = true;
            }

            concreteContext.Username = context.User.FindFirst(ClaimTypes.Name)?.Value
                                       ?? string.Empty;
        }

        await next(context);
    }
}