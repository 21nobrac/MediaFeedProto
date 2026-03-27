using System.Collections;
using MediaFeedProto;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

Dictionary<string, User> users = new() 
{
    {"Greg", new("Greg", "pass")},
    {"Carbon", new("Carbon", "A form of!")},
};

Dictionary<string, User> activeSessions = [];

List<Post> posts = [new("Hey", "Greg", "My First Post!", "atotallyuniqueID_I_SWEAR")];
Dictionary<string, Queue<Post>> feeds = [];



app.MapPost("/account/sign_in", ([FromForm] string username, [FromForm] string password, HttpContext ctx) =>
{
    if (users.TryGetValue(username, out var user) && user.Password == password)
    {
        var sessionID = Guid.NewGuid().ToString();
        activeSessions.Add(sessionID, user);

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

app.MapGet("/account/get_status", (HttpContext ctx) =>
{
    var sessionID = ctx.Request.Cookies["session_id"];
    string html;
    if (sessionID == null || !activeSessions.TryGetValue(sessionID, out var user))
        html = Views.SignIn;
    else
        html = Views.SignedInHeader(user.Username);
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

app.MapPost("/feed/build/", () =>
{
    string feedID = Guid.NewGuid().ToString();
    Queue<Post> feed = new();
    foreach (var post in posts.Shuffle())
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
            newPosts += ExamplePosts.BuildTextPost(next.Username, next.Title, next.Body, next.ID);
            newPosts += "\n";
        }
    }
    return Results.Content(newPosts, "text/html");
});

app.MapPost("/create/post", ([FromForm]string title, [FromForm]string body, HttpContext ctx) =>
{
    var sessionID = ctx.Request.Cookies["session_id"];
    if (sessionID == null || !activeSessions.TryGetValue(sessionID, out var user))
        return Results.Unauthorized();
    string postID = Guid.NewGuid().ToString();
    Post postRecord = new(title, user.Username, body, postID);
    posts.Add(postRecord);
    string post = ExamplePosts.BuildTextPost(user.Username, title, body, postID);
    return Results.Content(post, "text/html");
}).DisableAntiforgery();

app.Run();
