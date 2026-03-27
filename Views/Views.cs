namespace MediaFeedProto;
public static class Views
{
    public static string FeedPostGetter(string feedID) => $"""
    <div id="feed_filler"
        hx-post="/feed/{feedID}/next/10"
        hx-target="#posts"
        hx-trigger="load, load_more_posts"
        hx-swap="beforeend">
    </div>
    """;

    public const string SignIn = """
    <form class="sign_in" hx-post="/account/sign_in" hx-swap="outerHTML">
        <p>Username:</p>
        <input type="text" name="username">
        <p>Password:</p>
        <input type="text" name="password">
        <button type="submit">Sign In</button>
    </form>
    """;

    public static string SignedInHeader(string username) => $"""
        <div class="user_header">
            <p>Signed in as: {username}</p>
        </div>
    """;
}