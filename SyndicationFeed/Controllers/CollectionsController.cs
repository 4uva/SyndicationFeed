using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.Storage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SyndicationFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CollectionsController : ControllerBase
    {
        public CollectionsController(Repository repository)
        {
            this.repository = repository;
        }

        // GET api/collections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Collection>>> Get()
        {
            var collections = await repository.GetAllCollectionsAsync();
            return Ok(collections);
        }

        // GET api/collections/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Collection>> Get(long id)
        {
            var item = await repository.TryFindCollectionAsync(id);
            if (item != null)
                return item;
            else
                return NotFound($"Collection id {id} doesn't exist");
        }

        // POST api/collections
        [HttpPost]
        public async Task<ActionResult<Collection>> Post([FromBody] string name)
        {
            var newCollection = await repository.AddNewCollectionAsync(name);
            return CreatedAtAction(nameof(Get), new { id = newCollection.Id }, newCollection);
        }

        // DELETE api/collections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (await repository.TryRemoveCollectionAsync(id))
                return NoContent();
            else
                return NotFound($"Collection id {id} doesn't exist");
        }

        // GET api/collections/ids
        [HttpGet("ids")]
        public async Task<ActionResult<List<long>>> GetIds()
        {
            var collections = await repository.GetAllCollectionsAsync();
            var ids = collections.Select(coll => coll.Id).ToList();
            return Ok(ids);
        }

        readonly Repository repository;
    }
}
