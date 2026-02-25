using System.ComponentModel.DataAnnotations;

namespace BibliotecaVirtual.Shared
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La reseña es obligatoria")]
        [StringLength(250)]
        public string Reseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(100)]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de páginas es obligatorio")]
        [Range(1, 9999, ErrorMessage = "El número de páginas debe ser válido")]
        public int NoPaginas { get; set; }


        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Leido { get; set; } = true;

        [Required(ErrorMessage = "La calificación es obligatoria")]
        [Range(1.0, 3.0, ErrorMessage = "La calificación debe estar entre 1 y 3")]
        public int Calificación { get; set; }

        public string? ImagenUrl { get; set; }
    }
}
