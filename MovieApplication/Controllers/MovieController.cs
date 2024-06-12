using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using MovieApplication.Models;
using MovieApplication.ViewModel;
using System.Security.Claims;


namespace MovieApplication.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly MovieContext _context; 
        private readonly Cloudinary _cloudinary;
        public MovieController(MovieContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: MovieList
        public IActionResult List(int currentPg = 1)
        {
            const int pageSize = 4;

            if (currentPg < 1)
            {
                currentPg = 1;
            }

            var data1 = _context.MovieTable.ToList();

            // Map the data from MovieTable to MovieModelNew
            List<MovieModelNew> movieList = data1.Select(item => new MovieModelNew
            {
                Id = item.Id,
                MovieName = item.MovieName,
                Director = item.Director,
                Description = item.Description,
                Duration = item.Duration,
                Rating = item.Rating,
                Genre = int.Parse(item.Genre),
                Image = item.ImageUrl
            }).ToList();

            
            int totalCount = movieList.Count();
            var pagination = new Pagination(totalCount, currentPg, pageSize);
            int skip = (currentPg - 1) * pageSize;

           
            var paginatedMovies = movieList.Skip(skip).Take(pagination.PageSize).ToList();

            // Join paginated movies with genres
            var query = (from movie in paginatedMovies
                         join genre in _context.GenreTable
                         on movie.Genre equals genre.GenreId
                         select new responseModel
                         {
                             Id = movie.Id,
                             MovieName = movie.MovieName,
                             Description = movie.Description,
                             Director = movie.Director,
                             Duration = movie.Duration,
                             Genre = genre.Name,
                             Rating = movie.Rating,
                             Image = movie.Image
                         }).ToList();

            ViewBag.Pagination = pagination;

            return View(query);
        }


        //Http:Post/List
        [HttpPost]
        public IActionResult List(string SearchString, int currentPg = 1)
        {
            const int pageSize = 4;

            if (currentPg < 1)
            {
                currentPg = 1;
            }

           
            var data1 = _context.MovieTable.ToList();

            
            if (!string.IsNullOrEmpty(SearchString))
            {
                
                data1 = data1.Where(x => x.MovieName.Contains(SearchString)).ToList();

                // Map the search results to responseModel
                var searchQuery = (from movie in data1
                                   join genre in _context.GenreTable
                                   on int.Parse(movie.Genre) equals genre.GenreId
                                   select new responseModel
                                   {
                                       Id = movie.Id,
                                       MovieName = movie.MovieName,
                                       Description = movie.Description,
                                       Director = movie.Director,
                                       Duration = movie.Duration,
                                       Genre = genre.Name,
                                       Rating = movie.Rating,
                                       Image = movie.ImageUrl
                                   }).ToList();

                // Return all search results without pagination
                ViewBag.SearchString = SearchString;
                return View(searchQuery);
            }

            // Map the data from MovieTable to MovieModelNew
            List<MovieModelNew> movieList = data1.Select(item => new MovieModelNew
            {
                Id = item.Id,
                MovieName = item.MovieName,
                Director = item.Director,
                Description = item.Description,
                Duration = item.Duration,
                Rating = item.Rating,
                Genre = int.Parse(item.Genre),
                Image = item.ImageUrl
            }).ToList();

           
            int totalCount = movieList.Count();
            var pagination = new Pagination(totalCount, currentPg, pageSize);
            int skip = (currentPg - 1) * pageSize;

            
            var paginatedMovies = movieList.Skip(skip).Take(pagination.PageSize).ToList();

            // Join paginated movies with genres
            var query = (from movie in paginatedMovies
                         join genre in _context.GenreTable
                         on movie.Genre equals genre.GenreId
                         select new responseModel
                         {
                             Id = movie.Id,
                             MovieName = movie.MovieName,
                             Description = movie.Description,
                             Director = movie.Director,
                             Duration = movie.Duration,
                             Genre = genre.Name,
                             Rating = movie.Rating,
                             Image = movie.Image
                         }).ToList();

            ViewBag.Pagination = pagination;
            ViewBag.SearchString = SearchString; // To retain the search string in the view

            return View(query);
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.MovieTable.ToListAsync();
            var genres = await _context.GenreTable.ToListAsync();

            
            var movieDetail = (from movie in movies
                               join genre in genres
                               on movie.Genre equals genre.GenreId.ToString()
                               where movie.Id == id
                               select new responseModel
                               {
                                   Id = movie.Id,
                                   MovieName = movie.MovieName,
                                   Description = movie.Description,
                                   Director = movie.Director,
                                   Duration = movie.Duration,
                                   Genre = genre.Name,
                                   Rating = movie.Rating,
                                   Movie = movie.Movie,
                                   Image = movie.ImageUrl
                               }).FirstOrDefault();

            if (movieDetail == null)
            {
                return NotFound();
            }

            return View(movieDetail);

        }



        //Get:Review
        public async Task<IActionResult> Reviews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieModel = await _context.MovieTable
                .Include(m => m.Reviews)
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movieModel == null)
            {
                return NotFound();
            }

            ViewBag.AverageRating = movieModel.Ratings.Any() ? movieModel.Ratings.Average(r => r.RatingValue) : 0;
            ViewBag.LoggedInUserName = User.Identity.Name;

            return View(movieModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reviews(int movieId, string reviewContent, int ratingValue)
        {
            if (string.IsNullOrEmpty(reviewContent))
            {
                ModelState.AddModelError("", "Review content cannot be empty.");
                return RedirectToAction("Reviews", new { id = movieId });
            }

            var movie = await _context.MovieTable.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string loggedInUserName = User.Identity.Name;

            var comment = new ReviewModel
            {
                MovieId = movieId,
                UserId = loggedInUserId,
                UserName = loggedInUserName,
                Content = reviewContent,
                Rating=ratingValue,
                CreatedAt = DateTime.Now
            };

            var rating = new Ratings
            {
                MovieId = movieId,
                UserId = loggedInUserId,
                RatingValue = ratingValue
            };

            _context.Reviews.Add(comment);
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("Reviews", new { id = movieId });
        }


        // GET: Movie/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
           

            // Retrieve the list of genres from the database
            var genreList = await _context.GenreTable.ToListAsync();

            // GenreId=indicates which property of the objects in genreList should be used as the value for each dropdown option.
            //Name= indicates which property of the objects in genreList should be displayed as the text for each dropdown option.
            //additional data field can be useful for filter or sort the dropdown options based on the provided field.

            ViewBag.genres = new SelectList(genreList, "GenreId", "Name");
            return View();
        }


        //Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = string.Empty;
                string movieUrl = string.Empty;

                if (ViewModel?.Image != null && ViewModel.Image.Length > 0)
                {
                    var imageUploadResult = await UploadImageToCloudinary(ViewModel.Image);
                    if (imageUploadResult != null)
                    {
                        imageUrl = imageUploadResult.Url.ToString();
                    }
                }

                // Video upload to local storage
                if (ViewModel?.Movie != null && ViewModel.Movie.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/movies");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + ViewModel.Movie.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ViewModel.Movie.CopyToAsync(fileStream);
                    }

                    movieUrl = "/movies/" + uniqueFileName;
                }

                
                var existingMovie = await _context.MovieTable
                                                  .Where(x => x.MovieName == ViewModel.MovieName)
                                                  .SingleOrDefaultAsync();

                if (existingMovie != null)
                {
                    ViewBag.msg = "Cannot insert the same movie name twice.";
                    var genres = await _context.GenreTable.ToListAsync();
                    ViewBag.Genres = new SelectList(genres, "GenreId", "Name");
                    return View(ViewModel);
                }
                else
                {
                    var movieModel = new MovieModel
                    {
                        MovieName = ViewModel.MovieName,
                        Description = ViewModel.Description,
                        Director = ViewModel.Director,
                        Duration = ViewModel.Duration,
                        Genre = ViewModel.Genre,
                        Rating = ViewModel.Rating,
                        ImageUrl = imageUrl,
                        Movie = movieUrl
                    };

                    _context.MovieTable.Add(movieModel);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = "Movie added successfully!";
                    return RedirectToAction("List");
                }
            }

            return View(ViewModel);
        }


        // GET: Movie/Edit/5
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreList = await _context.GenreTable.ToListAsync();
            ViewBag.genres = new SelectList(genreList, "GenreId", "Name");



            var movieModel = await _context.MovieTable.FindAsync(id);
            if (movieModel == null)
            {
                return NotFound();
            }
            MovieViewModel vm = new MovieViewModel();
            string Path = string.Empty;
            
            vm.MovieName = movieModel.MovieName;
            vm.Description = movieModel.Description;
            vm.Director = movieModel.Director;
            vm.Duration = movieModel.Duration;
            vm.Rating = movieModel.Rating;
            vm.Genre = movieModel.Genre.ToString();
            Path= movieModel.ImageUrl;

           
            return View(vm);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MovieViewModel viewModel)
        {
            
            if (ModelState.IsValid)
            {                  

                
                var genre = await _context.GenreTable.ToListAsync();

                
               
                try
                {
                    var moviewUpdate = await _context.MovieTable.FindAsync(viewModel.Id);
                    if (moviewUpdate == null)
                        return NotFound();

                    string Path = string.Empty;
                    if (viewModel?.Image != null && viewModel.Image.Length > 0)
                    {
                        var uploadResult = await UploadImageToCloudinary(viewModel.Image);
                        if (uploadResult != null)
                        {
                           Path  = uploadResult.Url.ToString();
                        }
                    }
                   
                    moviewUpdate.Id = viewModel.Id;
                    moviewUpdate.MovieName = viewModel.MovieName;
                    moviewUpdate.Description = viewModel.Description;
                    moviewUpdate.Director = viewModel.Director;
                    moviewUpdate.Duration = viewModel.Duration;
                    moviewUpdate.Rating = viewModel.Rating;
                     moviewUpdate.ImageUrl= Path;

                    _context.MovieTable.Update(moviewUpdate);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = "Updated Successfully!";
                 

                }
                catch (DbUpdateConcurrencyException) // 2 users try to update db at same time this excpt. occurs
                {
                    if (!MovieModelExists(viewModel.Id)) // if the exception is not due to the movie model not being found, it throw to higher level excpt hadling.
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               return RedirectToAction("List");
               
            }
            return View(viewModel);
        }

        // Helper method to upload image to Cloudinary
        public async Task<ImageUploadResult> UploadImageToCloudinary(IFormFile imageFile)
        {
            using (var stream = imageFile.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageFile.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult;
            }
        }

        // GET: Movie/Delete/5
        public async Task<IActionResult> Delete(int? id) // return view(),ok(),NotFound()
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieModel = await _context.MovieTable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieModel == null)
            {
                return NotFound();
            }

            return View(movieModel);
        }

        // POST: Movie/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken] // include in payload and cross checks with server expected val.if matches req. proceeds,otherwise not
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieModel = await _context.MovieTable.FindAsync(id);
            if (movieModel != null)
            {
                _context.MovieTable.Remove(movieModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        private bool MovieModelExists(int id)
        {
            return _context.MovieTable.Any(e => e.Id == id);
        }
    }
}
