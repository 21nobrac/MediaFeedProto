using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace MediaFeedProto;

public class FeedService
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<Post>> feeds = new();

    /// <summary>
    /// Creates a new feed and returns its ID. The feed is a shuffled list of all posts in the database at the time of creation.
    /// </summary>
    /// <param name="db">The database context to use for retrieving posts.</param>
    /// <returns>The ID of the newly created feed. Used for future retrieval of posts from this feed.</returns>
    public async Task<string> CreateFeed(ApplicationDbContext db)
    {
        // Long-term TODO: look into better methods, 
        //   including grabbing a limited number of random posts from the database instead of all of them. 
        // Also user-specific feeds.
        var posts = (await db.Posts.ToListAsync()).Shuffle(); 

        var feed = new ConcurrentQueue<Post>(posts);
        string feedID;

        // Retry on id collision
        do { feedID = Guid.NewGuid().ToString(); }
        while (!feeds.TryAdd(feedID, feed));

        return feedID;
    }

    /// <summary>
    /// Retrieves the next batch of posts from the specified feed. If the feed ID is invalid or the feed is empty, returns an empty collection.
    /// </summary>
    /// <param name="feedID">The ID of the feed to retrieve posts from.</param>
    /// <param name="count">The desired number of posts to retrieve (may return none/fewer).</param>
    /// <returns>An enumerable of posts from the feed.</returns>
    public IEnumerable<Post> GetNextPosts(string feedID, int count)
    {
        if (feedID == null || !feeds.TryGetValue(feedID, out var feed))
            yield break;

        for (int i = 0; i < count; i++)
        {
            if (feed.TryDequeue(out var next))
                yield return next;
            else
            {
                feeds.TryRemove(feedID, out _); // Clean up empty feed -- Or should it generate new posts to it?
                yield break;
            }

        }
    }

    /// <summary>
    /// DO NOT CALL FROM UNVERIFIED USERS. Attempts deleting the feed.
    /// </summary>
    /// <param name="feedID">ID of feed to delete.</param>
    /// <returns>Bool representing whether the feed was successfully deleted.</returns>
    public bool TryDeleteFeed(string feedID)
    {
        return feeds.TryRemove(feedID, out _);
    }
}
