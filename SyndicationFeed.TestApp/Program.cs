using System;
using System.Collections.Generic;
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
            await root.AddCollection("Sample");

            var updatedCollections = await root.GetAllCollections();
            Console.WriteLine("Updated collections: ");
            DisplayCollections(updatedCollections);
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
    }
}
