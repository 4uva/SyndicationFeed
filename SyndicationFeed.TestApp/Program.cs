using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;
using SyndicationFeed.SDK;

namespace SyndicationFeed.TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var uri = new Uri("https://localhost");
            var port = 44301;
            var root = new SyndicationFeedRoot(uri, port);

            Console.WriteLine("Adding a sample collection");
            var coll = await root.AddCollection("Sample");
            var id1 = coll.Id;

            var allCollections = await root.GetAllCollections();
            Console.WriteLine("Collections:");
            DisplayCollections(allCollections);

            var addresses = new[]
            {
                new Uri("https://blogs.microsoft.com/feed/"),
                new Uri("https://news.microsoft.com/feed/"),
                new Uri("https://educationblog.microsoft.com/feed")
            };

            Console.WriteLine("Adding feeds");
            List<Feed> feeds = new List<Feed>();
            foreach (var address in addresses)
            {
                var feed = await coll.AddFeed(FeedType.Rss, address);
                feeds.Add(feed);
            }

            Console.WriteLine("Obtaining total feed");
            var totalFeed = await coll.GetTotalFeed();
            Console.WriteLine("Obtained:");
            DisplayFeed(totalFeed);

            Console.WriteLine("Removing feed 1");
            await coll.DeleteFeed(feeds[0].Id);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey(false);
        }

        static void DisplayCollections(IEnumerable<Collection> colls)
        {
            foreach (var coll in colls)
                DisplayCollection(coll);
        }

        static void DisplayCollection(Collection coll)
        {
            Console.WriteLine($"collection {coll.Name}, Id = {coll.Id}");
        }

        static void DisplayFeed(Feed feed)
        {
            Console.WriteLine($"feed {feed.Id}, {feed.Type}, has {feed.Publications.Count} publications");
            foreach (var pub in feed.Publications.Take(10))
                DisplayPublication(pub);
            if (feed.Publications.Count > 10)
                Console.WriteLine("[rest skipped]");
        }

        static void DisplayPublication(Publication publication)
        {
            Console.WriteLine($"publication {publication.Title}");

            var summary = publication.Summary;
            if (summary != null)
            {
                if (summary.Length > 40)
                    summary = summary.Substring(0, 40) + "...";
                Console.WriteLine($"summary: {summary}");
            }

            var content = publication.Content;
            if (content != null)
            {
                if (content.Length > 80)
                    content = content.Substring(0, 80) + "...";
                Console.WriteLine($"content: {content}");
            }
        }
    }
}
