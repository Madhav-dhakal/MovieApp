namespace MovieApplication.Models
{
    public class Ratings
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserName { get; set; }

        public int RatingValue { get; set; }
    }
}
