using System.Collections.Generic;
using System.Threading.Tasks;
using HelloMongoDb.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HelloMongoDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public IMongoDatabase Context { get; }
        public IMongoCollection<Book> Collection { get; }
        public IBookstoreDatabaseSettings Settings { get; }

        public BookController(IMongoDatabase context, IBookstoreDatabaseSettings settings)
        {
            Context = context;
            Settings = settings;
            Collection = Context.GetCollection<Book>(Settings.BooksCollectionName);
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            var result = await Collection
                .Find(e => true).ToListAsync();
            return Ok(result);
        }

        // GET: api/Book/5
        [HttpGet("{id}", Name = nameof(Get))]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var result = await Collection
                .Find(e => e.Id == id).SingleOrDefaultAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }
 
        // POST: api/Book
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Book value)
        {
            await Collection
                .InsertOneAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book value)
        {
            await Collection
                .FindOneAndReplaceAsync(e => e.Id == id, value);
            var result = await Collection
                .Find(e => e.Id == id).SingleOrDefaultAsync();
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await Collection.DeleteOneAsync(e => e.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
