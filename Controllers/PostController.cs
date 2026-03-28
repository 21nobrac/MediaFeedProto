using Microsoft.AspNetCore.Mvc;
using MediaFeedProto.Filters;
using Microsoft.EntityFrameworkCore;

namespace MediaFeedProto.Controllers;
public class PostController(ApplicationDbContext db) : Controller
{
    [HttpPost("/post/create/")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CreatePost([FromForm]string title, [FromForm]string body, HttpContext ctx, SessionService sessionService)
    {
        var sessionID = ctx.Request.Cookies["session_id"];
        var user = sessionID != null ? await sessionService.ValidateSession(sessionID, db) : null;
        if (user == null)
            return Unauthorized();
        string postID = Guid.NewGuid().ToString();
        Post postRecord = new Post { Title = title, Username = user.Username, Body = body, ID = postID };
        await db.Posts.AddAsync(postRecord);
        await db.SaveChangesAsync();
        string post = Views.BuildTextPost(user.Username, title, body, postID);
        return Content(post, "text/html");
    }

    [HttpPost("/post/{postID}/delete")]
    public async Task<IActionResult> DeletePost(string postID, HttpContext ctx, SessionService sessionService)
    {
        var sessionID = ctx.Request.Cookies["session_id"];
        var user = sessionID != null ? await sessionService.ValidateSession(sessionID, db) : null;
        if (user == null)
            return Unauthorized();
        
        var post = await db.Posts.FirstOrDefaultAsync(p => p.ID == postID);
        if (post == null)
            return NotFound();
        
        if (post.Username != user.Username)
            return Forbid();

        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return Ok();
    }
}