using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ToDoListContext _context;
        public TodoController(ToDoListContext context)
        {
            _context = context;
        }
 
        // GET: api/<TodoController>
        [HttpGet]
        public ActionResult<IEnumerable<ToDoListTable>> Get()
        {
            return _context.ToDoListTables.ToList();
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public ActionResult<ToDoListTable> Get(int id)
        {
            var ListOne = _context.ToDoListTables.Find(id);

            if(ListOne == null)
            {
                return NotFound("找不到資源");
            }
            else
            {
                return Ok(ListOne);
            }
        }

        // POST api/<TodoController>
        [HttpPost]
        public ActionResult<ToDoListTable> Post([FromBody] ToDoListTable value)
        {
            _context.ToDoListTables.Add(value);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get) , new { id = value.Id }, value);
        }

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ToDoListTable value)
        {
            if(id != value.Id)
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if(!_context.ToDoListTables.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "存取發生錯誤");
                }
            }
            return NoContent();
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var ListOne = _context.ToDoListTables.Find(id);
            if(ListOne == null)
            {
                return NotFound();
            }
            _context.ToDoListTables.Remove(ListOne);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
