namespace MediaFeedProto;

public class Post
{
    public string ID { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public Post() { }

    public Post(string title, string username, string body, string id)
    {
        Title = title;
        Username = username;
        Body = body;
        ID = id;
    }
}