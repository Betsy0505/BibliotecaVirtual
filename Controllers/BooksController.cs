using BibliotecaVirtual.Data;
using BibliotecaVirtual.Shared;
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
        public async Task<ActionResult<Book>> GetBook(int id)
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
        public async Task<ActionResult<Book>> PostBook([FromForm] Book book, IFormFile? file)
        {
            // 1. Si hay un archivo, lo guardamos físicamente
            if (file != null && file.Length > 0)
            {
                // Creamos la ruta de la carpeta wwwroot/uploads
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Generamos un nombre único para que no se sobrescriban fotos con el mismo nombre
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 2. Guardamos LA RUTA en la propiedad del objeto que irá a la BD
                book.ImagenUrl = "/uploads/" + fileName;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT api/<BooksController>/5 -> actualizar un libro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, [FromForm] Book book, IFormFile? file)
        {
            if (id != book.Id) return BadRequest();

            var libroExistente = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (libroExistente == null) return NotFound();

            if (file != null)
            {
                // Lógica para guardar la nueva foto (igual que en el POST)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }
                book.ImagenUrl = "/uploads/" + fileName;
            }
            else
            {
                // Si no subió foto nueva, mantenemos la anterior
                book.ImagenUrl = libroExistente.ImagenUrl;
            }

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
