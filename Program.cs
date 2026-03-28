using System.Collections;
using MediaFeedProto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=mediafeed.db"));

builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<FeedService>();

builder.Services.AddControllersWithViews();

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
app.MapControllers();

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

app.MapPost("/feed/build/", async (ApplicationDbContext db, FeedService feedService) =>
{
    string feedID = await feedService.CreateFeed(db);
    var html = Views.FeedPostGetter(feedID);

    return Results.Content(html, "text/html");
});

app.MapPost("/feed/{feedID}/next/{count}", (string feedID, int count, FeedService feedService) =>
{
    List<Post> posts = [.. feedService.GetNextPosts(feedID, count)];
    string newPosts = "";
    foreach (var post in posts)
    {
        newPosts += Views.BuildTextPost(post.Username, post.Title, post.Body, post.ID);
        newPosts += "\n";
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
