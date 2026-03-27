using System.Collections;
using MediaFeedProto;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

Dictionary<string, User> users = new() { {"Greg", new("Greg", "pass")}};

Dictionary<string, User> activeSessions = [];

List<Post> posts = [new("Hey", "Greg", "My First Post!", "atotallyuniqueID_I_SWEAR")];
Dictionary<string, Queue<Post>> feeds = [];



app.MapGet("/account/sign_in", (string username, string password) =>
{
    if (users.TryGetValue(username, out var user) && user.Password == password)
    {
        var sessionID = Guid.NewGuid().ToString();
        
    }
    
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

app.MapPost("/create/post", ([FromForm]string title, [FromForm]string username, [FromForm]string body) =>
{
    string postID = Guid.NewGuid().ToString();
    Post postRecord = new(title, username, body, postID);
    posts.Add(postRecord);
    string post = ExamplePosts.BuildTextPost(username, title, body, postID);
    return Results.Content(post, "text/html");
}).DisableAntiforgery();

app.Run();
