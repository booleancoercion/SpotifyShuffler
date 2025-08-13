namespace booleancoercion.SpotifyShuffler.Util;

public class TraceId
{
    private static readonly char[] s_chars = "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

    public string Id { get; private set; }

    public TraceId()
    {
        Id = string.Join("", Random.Shared.GetItems(s_chars, 6));
    }

    public TraceId(string traceId)
    {
        Id = traceId;
    }

    public override string ToString()
    {
        return $"Tr-{Id}";
    }
}
