### Twitter Terminator
----
C# dotnet Console application to assist with deleting all tweets from your account.

How to use (instructions for dotnet developers):

1. Download your twitter archive
1. <b>IMPORTANT</b>: <u><i>DO NOT</i></u> continue until step #1 is completed
1. Copy the file from your archive with the name "tweet-headers.js" into the TweetData folder
1. Remove the leading text in the file to the first [ (square bracket).  This makes the data a valid json array.
1. Change the name of the file "tweet-headers.js" to "tweet-headers.json"
1. While logged into your twitter account, open another page to the twitter Api developer portal. s/b this Url: https://developer.twitter.com/en/portal/dashboard
1. Follow instructions to create a free access level
1. Create an app in the default project
1. Edit user authentication settings for the app and set them to "Read and write"
1. Set the Type of App to "Native App"
1. If prompted for the 250 character description of the use cases, either type something with words or use lorem ipsum to generate something
1. For the Callback Uri and Website Url, enter something like "https://localhost:3456", and "https://test.com"
1. Go to the Keys and Tokens area and copy these values into the application.json file.
    - ApiKey
    - ApiSecret
    - AccessToken
    - AccessTokenSecret
1. Use VSCode or Visual Studio or JetBrains Rider to run the console application.

The process will take some time to loop through all twitter ids from your data, and delete each tweet one at a time.  If there is any tweet you want to save, locate the id from the Url of the tweet, in the data file and remove it from the data file.


