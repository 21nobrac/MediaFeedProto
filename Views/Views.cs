namespace MediaFeedProto;
public static class Views
{
    public static string FeedPostGetter(string feedID) => $"""
    <div id="feed_filler"
        hx-post="/feed/{feedID}/next/3"
        hx-target="#posts"
        hx-trigger="load, load_more_posts"
        hx-swap="beforeend">
    </div>
    """;
}