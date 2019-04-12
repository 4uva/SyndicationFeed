using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using SyndicationFeed.Models.Storage;
using SyndicationFeed.Common.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SyndicationFeed.Controllers
{
    [Route("api/collections/{collid}/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedsController : ControllerBase
    {
        public FeedsController(Repository repository, ILogger<FeedsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        // GET api/collections/1/feeds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feed>>> Get(long collid)
        {
            logger.LogInformation("Retrieving all feeds from collection id {id} started", collid);
            var feeds = await repository.TryFindFeedsAsync(collid);
            if (feeds != null)
            {
                logger.LogInformation("Retrieving all feeds from collection id {id} succeded", collid);
                return Ok(feeds);
            }
            else
            {
                logger.LogWarning("Retrieving all feeds from collection id {id} failed, no such id", collid);
                return NotFound($"Collection id {collid} doesn't exist");
            }
        }

        // GET api/collections/1/feeds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feed>> Get(long collid, long id)
        {
            logger.LogInformation("Retrieving feeds id {id} from collection id {collid} started", id, collid);
            var feed = await repository.TryFindFeedAsync(collid, id);
            if (feed != null)
            {
                logger.LogInformation("Retrieving feeds id {id} from collection id {collid} succeeded", id, collid);
                return Ok(feed);
            }
            else
            {
                logger.LogWarning("Retrieving feeds id {id} from collection id {collid} failed", id, collid);
                return NotFound($"Collection id {collid} or feed id {id} doesn't exist");
            }
        }

        // GET api/collections/1/feeds/all
        [HttpGet("all")]
        public async Task<ActionResult<Feed>> GetTotal(long collid)
        {
            logger.LogInformation("Retrieving combined feed from collection id {id} started", collid);
            var feeds = await repository.TryFindFeedsAsync(collid);
            if (feeds == null)
            {
                logger.LogWarning("Retrieving combined feed from collection id {id} failed, collection doesn't exist", collid);
                return NotFound($"Collection id {collid} doesn't exist");
            }

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

            logger.LogInformation("Retrieving combined feed from collection id {id} succeeded", collid);
            return Ok(totalFeed);
        }

        // POST api/collections/1/feeds
        [HttpPost]
        public async Task<ActionResult<Feed>> Post(long collid, [FromBody] Feed feed)
        {
            logger.LogInformation("Adding feed addr = {feed} to collection id {id} started", feed.SourceAddress, collid);
            if (feed.Type == FeedType.Virtual)
            {
                // report an error
                logger.LogWarning("Adding virtual feed to collection id {id} not possible", collid);
                return BadRequest("Cannot add virtual feed");
            }
            var newFeed = await repository.AddNewFeedAsync(collid, feed.Type, feed.SourceAddress);
            logger.LogInformation("Adding feed addr = {feed} to collection id {id} succeeded", feed.SourceAddress, collid);
            return CreatedAtAction(nameof(Get), new { collid, id = newFeed.Id }, newFeed);
        }

        // DELETE api/collections/1/feeds/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long collid, long id)
        {
            logger.LogInformation("Deleting feed id = {id} to collection id {collid} started", id, collid);
            if (await repository.TryRemoveFeedAsync(collid, id))
            {
                logger.LogInformation("Deleting feed id = {id} to collection id {collid} succeeded", id, collid);
                return NoContent();
            }
            else
            {
                logger.LogWarning("Deleting feed id = {id} to collection id {collid} failed", id, collid);
                return NotFound($"Collection id {collid} or feed id {id} doesn't exist");
            }
        }

        // GET api/collections/1/feeds/ids
        [HttpGet("ids")]
        public async Task<ActionResult<List<long>>> GetIds(long collid)
        {
            logger.LogInformation("Getting feed ids for collection id {collid} started", collid);
            var feeds = await repository.TryFindFeedsAsync(collid);
            if (feeds == null)
            {
                logger.LogWarning("Getting feed ids for collection id {collid} failed, no such collection", collid);
                return NotFound();
            }

            var ids = feeds.Select(feed => feed.Id).ToList();
            logger.LogInformation("Getting feed ids for collection id {collid} succeeded", collid);
            return Ok(ids);
        }

        readonly Repository repository;
        readonly ILogger logger;
    }
}
