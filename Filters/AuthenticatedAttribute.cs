using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MediaFeedProto.Filters;

public class AuthenticatedAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var services = context.HttpContext.RequestServices;
        var sessionService = services.GetRequiredService<SessionService>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        var sessionId = context.HttpContext.Request.Cookies["session_id"];
        var user = sessionId != null ? await sessionService.ValidateSession(sessionId, db) : null;

        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Stash the user so the action can access it without re-querying
        context.HttpContext.Items["user"] = user;
        await next();
    }
}