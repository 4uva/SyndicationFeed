using System.Collections.Generic;
using SyndicationFeed.Models.Storage;
using SyndicationFeed.Common.Models;
using Microsoft.AspNetCore.Mvc;

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
                return NotFound();
        }

        // GET api/collections/1/feeds/5
        [HttpGet("{id}")]
        public ActionResult<Feed> Get(long collid, long id)
        {
            var feed = repository.TryFindFeed(collid, id);
            if (feed != null)
                return Ok(feed);
            else
                return NotFound();
        }
  
        [HttpPost]
        public ActionResult<Feed> Post(long collid, [FromBody] Feed feed)
        {
            var newFeed = repository.AddNewFeed(collid, feed.Type, feed.SourceAddress);
            return CreatedAtAction(nameof(Get), new { collid, id = newFeed.Id }, newFeed);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long collid, long id)
        {
            if (repository.TryRemoveFeed(collid, id))
                return NoContent();
            else
                return NotFound();
        }

        readonly Repository repository;
    }
}
