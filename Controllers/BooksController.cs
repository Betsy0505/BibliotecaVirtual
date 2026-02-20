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
        // [FromForm] permite recibir archivos y texto al mismo tiempo
        public async Task<ActionResult<Book>> Post([FromForm] Book book, IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                // 1. Definir la carpeta (asegúrate de que existe wwwroot/uploads)
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                // 2. Generar nombre único para evitar que se sobrescriban
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(folder, fileName);

                // 3. Guardar el archivo en el servidor
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 4. Guardar la ruta en la base de datos
                book.ImagenUrl = $"/uploads/{fileName}";
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        // PUT api/<BooksController>/5 -> actualizar un libro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] Book book, IFormFile? file)
        {
            if (id != book.Id) return BadRequest(new { mensaje = "El id no coincide" });

            // Si el usuario sube una nueva imagen
            if (file != null && file.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                book.ImagenUrl = $"/uploads/{fileName}";
            }

            _context.Entry(book).State = EntityState.Modified;

            // Evitamos que EF borre la imagen anterior si no enviamos una nueva en el formulario
            if (string.IsNullOrEmpty(book.ImagenUrl))
                _context.Entry(book).Property(x => x.ImagenUrl).IsModified = false;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { /* lógica de error */ }

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

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No se seleccionó archivo.");

            // Crear un nombre único para la imagen
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Retornamos la URL para guardarla luego en el libro
            var url = $"/uploads/{fileName}";
            return Ok(new { url });
        }
    }
}
