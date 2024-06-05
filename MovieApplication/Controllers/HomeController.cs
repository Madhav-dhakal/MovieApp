using Microsoft.AspNetCore.Mvc;
using MovieApplication.Models;
using MovieApplication.ViewModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieContext _context;

        public HomeController(ILogger<HomeController> logger, MovieContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.MovieTable
                .Include(m => m.Ratings)
                .ToListAsync();
            var genres = await _context.GenreTable.ToListAsync();

            var movieList = (from movie in movies
                             join genre in genres on int.Parse(movie.Genre) equals genre.GenreId
                             select new responseModel
                             {
                                 Id = movie.Id,
                                 MovieName = movie.MovieName,
                                 Description = movie.Description,
                                 Director = movie.Director,
                                 Duration = movie.Duration,
                                 Genre = genre.Name,
                                 Image = movie.ImageUrl,

                                 Rating = movie.Ratings.Any() ? movie.Ratings.Average(r => r.RatingValue) : 0.0
                             }).ToList();

           
            double overallAverageRating = movieList.Any() ? movieList.Average(m => m.Rating) : 0.0;

            
            ViewBag.averageRating = overallAverageRating;

            return View(movieList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
