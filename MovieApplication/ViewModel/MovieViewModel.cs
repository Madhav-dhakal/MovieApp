using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieApplication.ViewModel
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Rating { get; set; }
        public  IFormFile? Image { get; set; }
      
    }

    public class responseModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Image { get; set; }
    }


}
