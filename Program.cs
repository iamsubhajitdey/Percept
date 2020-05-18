using System;
using System.Collections.Generic;
using System.IO;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Stream = Tweetinvi.Stream;

namespace Percept
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("Public Perception.");
            string[] lines = File.ReadAllLines("Access.cred");
            Auth.SetUserCredentials(lines[0], lines[1], lines[2], lines[3]);

            string searchTerm = File.ReadAllLines("phrase.Searchterm")[0];
            //var tweets = Timeline.GetHomeTimeline();
            GetSearchResults(searchTerm);
            //GetFilteredStream();

        }

        public static void GetSearchResults(string searchTerm)
        {
            SearchTweetsParameters searchParameter = new SearchTweetsParameters(searchTerm)
            {
                Lang = LanguageFilter.English,
                MaximumNumberOfResults = 200,
                Until = new DateTime(2020, 05, 18)
            };

            var tweets = Search.SearchTweets(searchParameter);
            List<string> tweetStrings = new List<string>();
            foreach (var tweet in tweets)
            {
                tweetStrings.Add(tweet.FullText);
            }
            File.WriteAllLines("SearchTweets.txt", tweetStrings);
        }


        public static void GetFilteredStream(string searchTerm)
        {
            var stream = Stream.CreateFilteredStream();
            stream.AddTrack(searchTerm);

            int tweetCount = 0;
            List<string> tweets = new List<string>();

            stream.MatchingTweetReceived += (sender, currentTweet) =>
            {
                tweetCount++;
                tweets.Add(currentTweet.Tweet.FullText);
                //Console.WriteLine("A tweet containing 'tweetinvi' has been found; the tweet is '" + currentTweet.Tweet.FullText + "'");
                if (tweetCount >= 5)
                {
                    stream.StopStream();
                }
            };

            stream.StartStreamMatchingAllConditions();

            File.WriteAllLines("FilterTweets.txt", tweets);
        }

    }
}