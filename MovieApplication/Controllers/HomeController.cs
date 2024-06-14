using Microsoft.AspNetCore.Mvc;
using MovieApplication.Models;
using MovieApplication.ViewModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieContext _context;

        public HomeController(MovieContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery)
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

            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchTerms = searchQuery.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                movieList = movieList.Where(m =>
                    searchTerms.Any(term =>
                        m.MovieName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        m.Director.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        m.Genre.Contains(term, StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();
            }

            foreach (var movie in movieList)
            {
                var ratingsForMovie = _context.Ratings.Where(r => r.MovieId == movie.Id).ToList();
                int totalRatingsCount = ratingsForMovie.Count();
                int totalRatingsSum = ratingsForMovie.Sum(r => r.RatingValue);
                double averageRating = totalRatingsCount > 0 ? (double)totalRatingsSum / totalRatingsCount : 0.0;
                movie.Rating = averageRating;
            }

            return View(movieList);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
