using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using CodeHollow.FeedReader;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.FeedCache;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class RssAndAtomExpanderCommon
    {
        static HttpClient httpClient = new HttpClient();

        static TimeSpan defaultExpiryTime = TimeSpan.FromHours(1);

        Cache cache = new Cache();

        public async Task ExpandAsync(FeedWithDownloadTime feed)
        {
            var uri = feed.SourceAddress;

            // first check if present in cache
            (var cachedFeed, var needRedownload) = cache.TryGetFeed(uri);

            string newContent = null;
            DateTime downloadTime = DateTime.UtcNow;
            bool feedFailed = false;
            if (needRedownload)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    if (cachedFeed != null)
                        request.Headers.IfModifiedSince = cachedFeed.LastDownloadTime;
                    using (var response = await httpClient.GetAsync(uri))
                    {
                        if (cachedFeed == null ||
                            response.StatusCode != System.Net.HttpStatusCode.NotModified)
                        {
                            newContent = await response.Content.ReadAsStringAsync();
                        }
                        // else there are old publications and content is not modified
                        // so we can continue using it
                    }
                    feed.LoadFailureMessage = null;
                }
                catch (Exception ex)
                {
                    feed.LoadFailureMessage = $"Feed download failed: {ex.Message}";
                    feedFailed = true;
                }
            }

            if (!feedFailed)
            {
                if (newContent != null)
                {
                    try
                    {
                        var publications = ExtractPublicationsFromContent(newContent);
                        feed.Publications = publications;
                        feed.LastDownloadTime = downloadTime;
                        feed.ValidTill = downloadTime + defaultExpiryTime;
                    }
                    catch (Exception ex)
                    {
                        feed.LoadFailureMessage = $"Feed extraction failed: {ex.Message}";
                        feedFailed = true;
                    }
                }
                else
                {
                    // no new content so we have cached feed
                    feed.Publications = cachedFeed.Publications;
                    feed.LastDownloadTime = cachedFeed.LastDownloadTime;
                    feed.ValidTill = cachedFeed.ValidTill;
                }
            }

            if (feedFailed)
            {
                feed.LastDownloadTime = DateTime.MinValue;
                feed.Publications = null;
                cache.UncacheFeed(feed);
            }
            else
            {
                cache.CacheFeed(feed);
            }
        }

        List<Publication> ExtractPublicationsFromContent(string content)
        {
            var feed = FeedReader.ReadFromString(content);
            var publications = new List<Publication>();
            foreach (var item in feed.Items)
                publications.Add(ConvertToPublication(item));
            return publications;
        }

        Publication ConvertToPublication(FeedItem item)
        {
            return new Publication()
            {
                Id = item.Id,
                Title = item.Title,
                Summary = item.Description,
                Content = item.Content,
                PublishTime = item.PublishingDate,
                Link = TryParseUri(item.Link)
            };
        }

        Uri TryParseUri(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out var result))
                return result;
            else
                return null;
        }
    }
}
