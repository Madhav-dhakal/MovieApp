using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApplication.Models;

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
        public IFormFile? Movie { get; set; } 
        public  IFormFile? Image { get; set; }
      
    }

    public class responseModel
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Genre { get; set; }
        public double Rating { get; set; }
        public string Image { get; set; }
        public List<Ratings> Ratings { get; set; } = new List<Ratings>();
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();
        public string Movie { get; set; } = string.Empty;
    }
}



