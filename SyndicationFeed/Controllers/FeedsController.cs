using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using SyndicationFeed.Models.Storage;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Controllers
{
    [Route("api/collections/{collid}/[controller]")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        public FeedsController(Repository repository)
        {
            this.repository = repository;
        }

        // GET api/collections/1/feeds
        [HttpGet]
        public ActionResult<IEnumerable<Feed>> Get(long collid)
        {
            var feeds = repository.TryFindFeeds(collid);
            if (feeds != null)
                return Ok(feeds);
            else
                return NotFound($"Collection id {collid} doesn't exist");
        }

        // GET api/collections/1/feeds/5
        [HttpGet("{id}")]
        public ActionResult<Feed> Get(long collid, long id)
        {
            var feed = repository.TryFindFeed(collid, id);
            if (feed != null)
                return Ok(feed);
            else
                return NotFound($"Collection id {collid} or feed id {id} doesn't exist");
        }

        // GET api/collections/1/feeds/all
        [HttpGet("all")]
        public ActionResult<Feed> GetTotal(long collid)
        {
            var feeds = repository.TryFindFeeds(collid);
            if (feeds == null)
                return NotFound($"Collection id {collid} doesn't exist");

            var allPublications = new List<Publication>();
            foreach (var feed in feeds)
                allPublications.AddRange(feed.Publications);
            allPublications.Sort(CompareHelper.ComparePublicationsByDate);

            var totalFeed = new Feed()
            {
                Id = -1,
                SourceAddress = null,
                Type = FeedType.Virtual,
                Publications = allPublications
            };

            return Ok(totalFeed);
        }

        // POST api/collections/1/feeds
        [HttpPost]
        public ActionResult<Feed> Post(long collid, [FromBody] Feed feed)
        {
            if (feed.Type == FeedType.Virtual)
            {
                // report an error
                return BadRequest("Cannot add virtual feed");
            }
            var newFeed = repository.AddNewFeed(collid, feed.Type, feed.SourceAddress);
            return CreatedAtAction(nameof(Get), new { collid, id = newFeed.Id }, newFeed);
        }

        // DELETE api/collections/1/feeds/1
        [HttpDelete("{id}")]
        public IActionResult Delete(long collid, long id)
        {
            if (repository.TryRemoveFeed(collid, id))
                return NoContent();
            else
                return NotFound($"Collection id {collid} or feed id {id} doesn't exist");
        }

        // GET api/collections/1/feeds/ids
        [HttpGet("ids")]
        public ActionResult<List<long>> GetIds(long collid)
        {
            var feeds = repository.TryFindFeeds(collid);
            if (feeds == null)
                return NotFound();

            var ids = feeds.Select(feed => feed.Id).ToList();
            return Ok(ids);
        }

        readonly Repository repository;
    }
}
