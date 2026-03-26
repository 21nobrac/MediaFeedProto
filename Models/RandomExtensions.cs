namespace MediaFeedProto;
public static class RandomExtensions
{
    public static T RandomElement<T>(this List<T> list, Random rng)
    {
        if (list.Count < 1) throw new InvalidOperationException("list cannot be empty");
        return list[rng.Next(list.Count)];
    }
}