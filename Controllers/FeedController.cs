using Microsoft.AspNetCore.Mvc;
using MediaFeedProto.Filters;

namespace MediaFeedProto.Controllers;
public class FeedController(FeedService feedService, ApplicationDbContext db) : Controller
{
    [HttpPost("/feed/build/")]
    public async Task<IActionResult> BuildFeed()
    {
        string feedID = await feedService.CreateFeed(db);
        var html = Views.FeedPostGetter(feedID);
        return Content(html, "text/html");
    }

    [HttpPost("/feed/{feedID}/next/{count}")]
    public IActionResult GetNextPosts(string feedID, int count)
    {
        List<Post> posts = [.. feedService.GetNextPosts(feedID, count)];
        string newPosts = "";
        foreach (var post in posts)
        {
            newPosts += Views.BuildTextPost(post.Username, post.Title, post.Body, post.ID);
            newPosts += "\n";
        }
        return Content(newPosts, "text/html");
    }
}