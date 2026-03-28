using Microsoft.AspNetCore.Mvc;
using MediaFeedProto.Filters;

namespace MediaFeedProto.Controllers;

public class AccountController(SessionService sessions, ApplicationDbContext db) : Controller
{
    [HttpGet("/account/get_status")]
    public async Task<IActionResult> GetStatus()
    {
        var sessionId = Request.Cookies["session_id"];
        var user = sessionId != null ? await sessions.ValidateSession(sessionId, db) : null;

        string html = user != null ? Views.SignedInHeader(user.Username) : Views.SignIn;
        return Content(html, "text/html");
    }

    [HttpPost("/account/sign_in")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SignIn([FromForm] string username, [FromForm] string password)
    {
        var user = await UserValidation.TryGetUser(username, password, db);
        if (user == null) return Unauthorized();

        var sessionId = sessions.CreateSession(user);
        Response.Cookies.Append("session_id", sessionId, new CookieOptions
        {
            HttpOnly = true,
            Secure = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development",
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });

        return Content(Views.SignedInHeader(user.Username), "text/html");
    }
}