using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.Storage;

using Microsoft.AspNetCore.Mvc;

namespace SyndicationFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        public CollectionsController(Repository repository)
        {
            this.repository = repository;
        }

        // GET api/collections
        [HttpGet]
        public ActionResult<IEnumerable<Collection>> Get()
        {
            var collections = repository.GetAllCollections();
            return Ok(collections);
        }

        // GET api/collections/1
        [HttpGet("{id}")]
        public ActionResult<Collection> Get(long id)
        {
            var item = repository.TryFindCollection(id);
            if (item != null)
                return item;
            else
                return NotFound();
        }

        // POST api/collections
        [HttpPost]
        public ActionResult<Collection> Post([FromBody] string name)
        {
            var newCollection = repository.AddNewCollection(name);
            return CreatedAtAction(nameof(Get), new { id = newCollection.Id }, newCollection);
        }

        // DELETE api/collections/5
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            if (repository.TryRemoveCollection(id))
                return NoContent();
            else
                return NotFound();
        }

        // GET api/collections/ids
        [HttpGet("ids")]
        public ActionResult<List<long>> GetIds()
        {
            var collections = repository.GetAllCollections();
            var ids = collections.Select(coll => coll.Id).ToList();
            return Ok(ids);
        }

        readonly Repository repository;
    }
}
