using System.ComponentModel.DataAnnotations;

namespace MovieApplication.Models
{
    public class Ratings
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
        public  string UserId { get; set; }
        public int RatingValue { get; set; }
    }
}
