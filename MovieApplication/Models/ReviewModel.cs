using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApplication.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]

        public MovieModel Movie { get; set; }
        public int Rating { get;set; }
        public string  UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
