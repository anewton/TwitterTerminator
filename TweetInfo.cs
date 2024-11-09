using System.Text.Json.Serialization;

namespace TwitterTerminator;

public class Tweet
{
    [JsonPropertyName("tweet")]
    public TweetInfo Info { get; set; }
}

public class TweetInfo
{
    [JsonPropertyName("tweet_id")]
    public string TweetId { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
}

public class DeletedTweet
{
    public long Id { get; set; }
}