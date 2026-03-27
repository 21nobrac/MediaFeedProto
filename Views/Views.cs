namespace MediaFeedProto;
public static class Views
{
    // Const views:
    public const string SignIn = """
    <form class="sign_in" hx-post="/account/sign_in" hx-swap="outerHTML">
        <p>Username:</p>
        <input type="text" name="username">
        <p>Password:</p>
        <input type="text" name="password">
        <button type="submit">Sign In</button>
    </form>
    """;

    // Generated views:
    public static string FeedPostGetter(string feedID) => $"""
    <div id="feed_filler"
        hx-post="/feed/{feedID}/next/10"
        hx-target="#posts"
        hx-trigger="load, load_more_posts"
        hx-swap="beforeend">
    </div>
    """;

    public static string SignedInHeader(string username) => $"""
        <div class="user_header">
            <p>Signed in as: {username}</p>
        </div>
    """;

    public static string BuildTextPost(string username, string title, string body, string postID) => $"""
    <div id="text_post_{postID}" class="post">
        <div class="post_titlecard">
            <h1>{title}</h1>
            <p>{username}</p>
        </div>
        <div class="post_body">
            <p>{body}</p>
        </div>
        <div class="post_interactions">
            <button
                hx-get="/post/{postID}/comments"
                hx-swap="innerhtml"
                hx-target="#comment_space_{postID}">
                Comments
            </button>
            <button>
                Like
            </button>
            <button>
                Dislike
            </button>
        </div>
        <div class="post_comments" id="comment_space_{postID}"></div>
    </div>
    """;
}