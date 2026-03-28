namespace MediaFeedProto.Models;
public class TextPostViewModel
{
    public required string Username { get; init; }
    public required string Title    { get; init; }
    public required string Body     { get; init; }
    public required string PostId   { get; init; }
}