using Microsoft.AspNetCore.Mvc;
using MediaFeedProto.Filters;

namespace MediaFeedProto.Controllers;
public class FeedController(FeedService feedService, ApplicationDbContext db) : Controller
{
    [HttpPost("/feed/build/")]
    public async Task<IActionResult> BuildFeed()
    {
        string feedID = await feedService.CreateFeed(db);
        return PartialView("FeedPostGetter", feedID);
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