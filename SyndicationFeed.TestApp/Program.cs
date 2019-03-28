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

            var initialCollections = await root.GetAllCollections();
            Console.WriteLine("Initial collections: ");
            DisplayCollections(initialCollections);

            Console.WriteLine("Adding a sample collection");
            var coll1 = await root.AddCollection("Sample");
            var id1 = coll1.Id;
            Console.WriteLine("Obtained back collection:");
            DisplayCollection(coll1);

            Console.WriteLine("Adding another collection");
            var coll2 = await root.AddCollection("Sample 2");
            var id2 = coll2.Id;
            Console.WriteLine("Obtained back collection:");
            DisplayCollection(coll2);

            Console.WriteLine("Deleting first collection");
            await root.DeleteCollection(id1);

            Console.WriteLine("Obtaining ids");
            var ids = await root.GetCollectionIds();

            if (!ids.Contains(id2))
                Console.WriteLine("Unexpected: all indices don't contain added index");
            if (ids.Contains(id1))
                Console.WriteLine("Unexpected: all indices contain deleted index");

            var updatedCollections = await root.GetAllCollections();
            Console.WriteLine("Updated collections: ");
            DisplayCollections(updatedCollections);

            Console.WriteLine("Getting collections one by one:");
            foreach (var id in ids)
            {
                var coll = await root.GetCollection(id);
                DisplayCollection(coll);
            }

            // now, coll2 should be present
            Console.WriteLine("Adding feed 1");
            var feed1 = await coll2.AddFeed(
                FeedType.Rss, new Uri("https://blogs.microsoft.com/feed/"));
            Console.WriteLine("Obtained back:");
            DisplayFeed(feed1);

            Console.WriteLine("Adding feed 2");
            var feed2 = await coll2.AddFeed(
                FeedType.Rss, new Uri("https://educationblog.microsoft.com/feed"));
            Console.WriteLine("Obtained back:");
            DisplayFeed(feed2);

            Console.WriteLine("Adding feed 3");
            var feed3 = await coll2.AddFeed(
                FeedType.Rss, new Uri("https://www.reddit.com/r/microsoft/.rss?format=xml"));
            Console.WriteLine("Obtained back:");
            DisplayFeed(feed3);

            Console.WriteLine("Removing feed 1");
            await coll2.DeleteFeed(feed1.Id);

            Console.WriteLine("Obtaining syndicated feed");
            var commonFeed = await coll2.GetSyndicatedFeed();
            Console.WriteLine("Obtained:");
            DisplayFeed(commonFeed);

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
