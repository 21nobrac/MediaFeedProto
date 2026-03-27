using System.Collections;
using MediaFeedProto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=mediafeed.db"));

builder.Services.AddSingleton<SessionService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    
    // Seed initial data if Users table is empty
    if (!db.Users.Any())
    {
        db.Users.Add(new User { Username = "Greg", Password = "pass" });
        db.Users.Add(new User { Username = "Carbon", Password = "A form of!" });
        db.SaveChanges();
    }
    
    // Seed initial post if Posts table is empty
    if (!db.Posts.Any())
    {
        db.Posts.Add(new Post { Title = "Hey", Username = "Greg", Body = "My First Post!", ID = "atotallyuniqueID_I_SWEAR" });
        db.SaveChanges();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();

List<Post> posts = [];
Dictionary<string, Queue<Post>> feeds = [];

app.MapPost("/account/sign_in", async ([FromForm] string username, [FromForm] string password, HttpContext ctx, ApplicationDbContext db, SessionService sessionService) =>
{
    var user = await UserValidation.TryGetUser(username, password, db);
    if (user != null)
    {
        var sessionID = sessionService.CreateSession(user);

        ctx.Response.Cookies.Append("session_id", sessionID, new CookieOptions
        {
            HttpOnly = true,
            Secure = !builder.Environment.IsDevelopment(),
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });

        return Results.Ok(Views.SignedInHeader(user.Username));
    }
    return Results.Unauthorized();
})
.DisableAntiforgery();

app.MapGet("/account/get_status", async (HttpContext ctx, ApplicationDbContext db, SessionService sessionService) =>
{
    var sessionID = ctx.Request.Cookies["session_id"];
    var user = sessionID != null ? await sessionService.ValidateSession(sessionID, db) : null;
    string html = user != null ? Views.SignedInHeader(user.Username) : Views.SignIn;
    return Results.Content(html, "text/html");
});

app.MapGet("/feed/random/{count}", (int count) =>
{
    string newPosts = "";
    for (int i = 0; i < count; i++)
    {
        newPosts += ExamplePosts.RandomTextPost();
        newPosts += "\n";
    }
    return Results.Content(newPosts, "text/html");
});

app.MapGet("/post/{postID}/comments", (string postID) =>
{
    string comment = ExamplePosts.RandomComment();
    return Results.Content(comment, "text/html");
});

app.MapPost("/feed/build/", async (ApplicationDbContext db) =>
{
    string feedID = Guid.NewGuid().ToString();
    Queue<Post> feed = new();
    foreach (var post in (await db.Posts.ToListAsync()).Shuffle())
    {
        feed.Enqueue(post);
    }
    feeds.Add(feedID, feed);
    var html = Views.FeedPostGetter(feedID);

    return Results.Content(html, "text/html");
});

app.MapPost("/feed/{feedID}/next/{count}", (string feedID, int count) =>
{
    Queue<Post> feed = feeds[feedID];
    string newPosts = "";
    for (int i = 0; i < count; i++)
    {
        if (feed.TryDequeue(out var next))
        {
            newPosts += Views.BuildTextPost(next.Username, next.Title, next.Body, next.ID);
            newPosts += "\n";
        }
    }
    return Results.Content(newPosts, "text/html");
});

app.MapPost("/create/post", async ([FromForm]string title, [FromForm]string body, HttpContext ctx, ApplicationDbContext db, SessionService sessionService) =>
{
    var sessionID = ctx.Request.Cookies["session_id"];
    var user = sessionID != null ? await sessionService.ValidateSession(sessionID, db) : null;
    if (user == null)
        return Results.Unauthorized();
    string postID = Guid.NewGuid().ToString();
    Post postRecord = new Post { Title = title, Username = user.Username, Body = body, ID = postID };
    await db.Posts.AddAsync(postRecord);
    await db.SaveChangesAsync();
    string post = Views.BuildTextPost(user.Username, title, body, postID);
    return Results.Content(post, "text/html");
}).DisableAntiforgery();

app.MapPost("/post/{postID}/delete", async (string postID, HttpContext ctx, ApplicationDbContext db, SessionService sessionService) =>
{
    var sessionID = ctx.Request.Cookies["session_id"];
    var user = sessionID != null ? await sessionService.ValidateSession(sessionID, db) : null;
    if (user == null)
        return Results.Unauthorized();
    
    var post = await db.Posts.FirstOrDefaultAsync(p => p.ID == postID);
    if (post == null)
        return Results.NotFound();
    
    if (post.Username != user.Username)
        return Results.Forbid();

    db.Posts.Remove(post);
    await db.SaveChangesAsync();
    return Results.Ok();
}).DisableAntiforgery();

app.Run();
