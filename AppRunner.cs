using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using Tweetinvi;

namespace TwitterTerminator;

internal class AppRunner(ISettings settings, ILogger<AppRunner> logger)
{
    internal async Task RunAllAsync()
    {
        int deletedTweetCount = 0;
        List<long> previouslyDeleted = ReadDeleteListFromFile();

        try
        {
            var tweetIdList = GetTweetIdList();

            var client = new TwitterClient(
                settings.TwitterApiSettings.ApiKey,
                settings.TwitterApiSettings.ApiSecret,
                settings.TwitterApiSettings.AccessToken,
                settings.TwitterApiSettings.AccessTokenSecret);

            var user = await client.Users.GetAuthenticatedUserAsync();
            var tweetsClient = new Tweetinvi.Client.TweetsClient(client);

            foreach (var tweetId in tweetIdList)
            {
                try
                {
                    if (!previouslyDeleted.Contains(tweetId))
                    {
                        await tweetsClient.DestroyTweetAsync(tweetId);
                        deletedTweetCount++;
                        logger.LogInformation("Deleted {DeletedTweetCount} tweets.", deletedTweetCount);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "{TweetId} Error deleting tweet. {ErrorMessage}", tweetId, ex.Message);
                }
                previouslyDeleted.Add(tweetId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in twitter client. {ErrorMessage}", ex.Message);
        }

        previouslyDeleted = previouslyDeleted.GroupBy(x => x).Select(y => y.First()).ToList();
        WriteDeleteListToFile(previouslyDeleted);

        logger.LogInformation("Application runner is complete. Deleted {DeletedTweetCount} tweets.", deletedTweetCount);
    }

    private List<long> GetTweetIdList()
    {
        List<long> result = [];
        var tweetHeadersJsonStream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("TwitterTerminator.TweetData.tweet-headers.json");

        using StreamReader reader = new(tweetHeadersJsonStream);
        var tweetHeadersJson = reader.ReadToEnd();
        var tweetHeaders = JsonSerializer.Deserialize<List<Tweet>>(tweetHeadersJson);
        result = tweetHeaders.OrderBy(tweet => _twitterDateParser(tweet.Info.CreatedAt)).Select(tweet => long.Parse(tweet.Info.TweetId)).ToList();
        return result;
    }

    private Func<string, DateTime> _twitterDateParser = (twitterDateString) => DateTime.ParseExact(
        twitterDateString,
        "ddd MMM dd HH:mm:ss K yyyy",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.AdjustToUniversal);

    private static void WriteDeleteListToFile(List<long> deletedTweetIds)
    {
        var directory = AppContext.BaseDirectory;
        var fileName = "DeletedTweets.json";
        var filePath = Path.Combine(directory, fileName);
        List<DeletedTweet> alreadyDeletedTweets = [];
        if (File.Exists(filePath))
        {
            var fileContents = File.ReadAllText(filePath);
            alreadyDeletedTweets = JsonSerializer.Deserialize<List<DeletedTweet>>(fileContents);
        }
        foreach (var tweetId in deletedTweetIds)
        {
            alreadyDeletedTweets.Add(new DeletedTweet() { Id = tweetId });
        }
        var deletedTweetsJson = JsonSerializer.Serialize(alreadyDeletedTweets);
        File.WriteAllText(filePath, deletedTweetsJson);
    }

    private static List<long> ReadDeleteListFromFile()
    {
        var directory = AppContext.BaseDirectory;
        var fileName = "DeletedTweets.json";
        var filePath = Path.Combine(directory, fileName);
        List<long> alreadyDeletedTweetIds = [];
        if (File.Exists(filePath))
        {
            var fileContents = File.ReadAllText(filePath);
            var alreadyDeletedTweets = JsonSerializer.Deserialize<List<DeletedTweet>>(fileContents);
            foreach (var deletedTweet in alreadyDeletedTweets)
            {
                alreadyDeletedTweetIds.Add(deletedTweet.Id);
            }
        }

        return alreadyDeletedTweetIds;
    }
}