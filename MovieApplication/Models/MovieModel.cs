namespace MovieApplication.Models
{
    public class MovieModel
    {
        
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Rating{ get; set; }
         public string ImageUrl { get; set; }=string.Empty;
    }

}
