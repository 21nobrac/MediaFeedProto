namespace MediaFeedProto;
public static class ExamplePosts
{
    private static readonly Random rng = new();

    private static readonly List<string> usernames = [
        "abstracteddutiful",
        "snowballprevent",
        "penaltyagain",
        "chorusprimarily",
        "woodthirsty",
        "pursertalk",
        "penetrategrey",
        "basilmelody",
        "stareablaze",
        "fruitfultipped",
        "fretlodestone",
    ];

    private static readonly List<string> titles = [
        "Blockbuster? No thanks.",
        "Feeling Proud",
        "Kinda Hungry",
        "YO YO WASSUP",
        "Ethereal Vibes.",
        "See y'all next year",
        "I can't believe it"
    ];

    private static readonly List<string> sentences = [
        "So long and thanks for the fish.",
        "Their argument could be heard across the parking lot.",
        "The beauty of the sunset was obscured by the industrial cranes.",
        "Everyone says they love nature until they realize how dangerous she can be.",
        "The heat.",
        "It's important to remember to be aware of rampaging grizzly bears.",
        "Courage and stupidity were all he had.",
        "She looked at the masterpiece hanging in the museum but all she could think is that her five-year-old could do better.",
        "Check back tomorrow; I will see if the book has arrived.",
        "Tomatoes make great weapons when water balloons aren't available.",
    ];

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

    private static string BuildComment(string username, string body) => $"""
    <div id="comment" class="comment">
        <div class="comment_nameplate">
            <p>{username}</p>
        </div>
        <div class="comment_body">
            <p>{body}</p>
        </div>
        <div class="comment_interactions">
            <button>
                Like
            </button>
            <button>
                Dislike
            </button>
        </div>
    </div>
    """;

    public static string RandomComment()
    {
        var username = usernames.RandomElement(rng);
        var body = sentences.RandomElement(rng);

        var html = BuildComment(username, body);

        return html;
    }

    public static string RandomTextPost()
    {
        var username = usernames.RandomElement(rng);
        var title = titles.RandomElement(rng);
        var body = $"{sentences.RandomElement(rng)} {sentences.RandomElement(rng)}"; // grab two random sentences
        
        var html = BuildTextPost(username, title, body, Guid.NewGuid().ToString());

        return html;
    }
}