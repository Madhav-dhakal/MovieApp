namespace MovieApplication.Models
{
    public class MovieModel
    {
        
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; }= string.Empty;      
        public int Rating{ get; set; }
         public string ImageUrl { get; set; }=string.Empty;
        public string Movie { get; set; } = string.Empty;
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
        public List<Ratings> Ratings { get; set; } = new List<Ratings>();

        
    }
    public class MovieModelNew
    {

        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int Genre { get; set; }
        public int Rating { get; set; }
        public string Image { get; set; } = string.Empty;
    }

}
