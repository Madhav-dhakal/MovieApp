using System.ComponentModel.DataAnnotations;

namespace MovieApplication.Models
{
    public class GenreModel
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }=string.Empty;
        

    }
}
