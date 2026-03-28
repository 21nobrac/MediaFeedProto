namespace MediaFeedProto;
public class Comment
{
    public string ID { get; set; } = string.Empty;
    public string PostID { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public Comment() { }

    public Comment(string postID, string username, string body, string id)
    {
        PostID = postID;
        Username = username;
        Body = body;
        ID = id;
    }
}

public static class CommentExtensions
{
    public static Models.CommentViewModel ToViewModel(this Comment comment)
    {
        return new Models.CommentViewModel
        {
            Username = comment.Username,
            Body = comment.Body
        };
    }
}