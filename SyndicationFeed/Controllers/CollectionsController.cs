using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.Storage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace SyndicationFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CollectionsController : ControllerBase
    {
        public CollectionsController(Repository repository, ILogger<CollectionsController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        // GET api/collections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collection>>> Get()
        {
            logger.LogInformation("Retrieving all collections");
            var collections = await repository.GetCollectionsAsync();
            return Ok(collections);
        }

        // GET api/collections/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> Get(long id)
        {
            logger.LogInformation("Retrieving collection by id {id} started", id);
            var item = await repository.TryFindCollectionAsync(id);
            if (item != null)
            {
                logger.LogInformation("Retrieving collection by id {id} succeeded", id);
                return item;
            }
            else
            {
                logger.LogWarning("Retrieving collection by id {id} failed, id doesn't exist", id);
                return NotFound($"Collection id {id} doesn't exist");
            }
        }

        // POST api/collections
        [HttpPost]
        public async Task<ActionResult<Collection>> Post([FromBody] string name)
        {
            logger.LogInformation("Adding collection (name = {name}) started", name);
            var newCollection = await repository.AddNewCollectionAsync(name);
            logger.LogInformation("Adding collection {name} succeeded", name);
            return CreatedAtAction(nameof(Get), new { id = newCollection.Id }, newCollection);
        }

        // DELETE api/collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            logger.LogInformation("Deleting collection id {id} started", id);
            if (await repository.TryRemoveCollectionAsync(id))
            {
                logger.LogInformation("Deleting collection id {id} succeeded", id);
                return NoContent();
            }
            else
            {
                logger.LogWarning("Deleting collection by id {id} failed, id doesn't exist", id);
                return NotFound($"Collection id {id} doesn't exist");
            }
        }

        // GET api/collections/ids
        [HttpGet("ids")]
        public async Task<ActionResult<List<long>>> GetIds()
        {
            logger.LogInformation("Retrieving collection ids started");
            var collections = await repository.GetCollectionsAsync();
            var ids = collections.Select(coll => coll.Id).ToList();
            logger.LogInformation("Retrieving collection ids succeeded");
            return Ok(ids);
        }

        readonly Repository repository;
        readonly ILogger logger;
    }
}
