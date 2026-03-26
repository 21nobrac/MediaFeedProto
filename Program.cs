using MediaFeedProto;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/feed/random/{count}", (int count) =>
{
    string posts = "";
    for (int i = 0; i < count; i++)
    {
        posts += ExamplePosts.RandomTextPost();
        posts += "\n";
    }
    return Results.Content(posts, "text/html");
});

app.MapGet("/post/{postID}/comments", (string postID) =>
{
    string comment = ExamplePosts.RandomComment();
    return Results.Content(comment, "text/html");
});

app.Run();
