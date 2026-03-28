using Microsoft.AspNetCore.Mvc;
using MediaFeedProto.Filters;

namespace MediaFeedProto.Controllers;
public class FeedController(FeedService feedService, ApplicationDbContext db) : Controller
{
    [HttpPost("/feed/build/{batchSize}")]
    public async Task<IActionResult> BuildFeed(int batchSize)
    {
        string feedID = await feedService.CreateFeed(db);
        var posts = feedService.GetNextPosts(feedID, batchSize)
            .Select(p => new Models.TextPostViewModel
            {
                Username = p.Username,
                Title    = p.Title,
                Body     = p.Body,
                PostId   = p.ID
            });
        return PartialView("BatchingFeed", (feedID, batchSize, posts));
    }

    [HttpPost("/feed/{feedID}/next/{count}")]
    public IActionResult GetNextPosts(string feedID, int count)
    {
        var posts = feedService.GetNextPosts(feedID, count)
            .Select(p => new Models.TextPostViewModel
            {
                Username = p.Username,
                Title    = p.Title,
                Body     = p.Body,
                PostId   = p.ID
            });

        return PartialView("NextPosts", posts);
    }
}