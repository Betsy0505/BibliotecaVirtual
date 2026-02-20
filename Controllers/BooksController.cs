using BibliotecaVirtual.Data;
using BibliotecaVirtual.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/<BooksController> -> obtener todos los libros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            return await _context.Books.ToListAsync();
        }

        // GET api/<BooksController>/5 -> obtener un libro por id 
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound(new { mensaje = "El libro no existe" });
            }

            return book;
        }

        // POST api/<BooksController> -> guardar un libro
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // vamos a retornar el libro creado y su ubicacion
            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        // PUT api/<BooksController>/5 -> actualizar un libro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Book book) {
            if (id != book.Id)
            {
                return BadRequest(new { mensaje = "El id no coinicide" });
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if(!_context.Books.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else { throw; }
            }
            return NoContent();
        }

        // DELETE api/<BooksController>/5 -> eliminar un libro 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if(book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = " Libro eliminado correctamente" } );

        }
    }
}
