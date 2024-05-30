using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using MovieApplication.Migrations;
using MovieApplication.Models;
using MovieApplication.ViewModel;
using NuGet.Protocol.Core.Types;


namespace MovieApplication.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly MovieContext _context; //for communication with db we create Moviecontext or ...
        private readonly Cloudinary _cloudinary;
        public MovieController(MovieContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: MovieList
        public IActionResult List(int currentPg = 1) // if the parameter is not provided when the method is called, it will default to 1.
        {
            // Retrieve the list of movies from the database
           var data1 = _context.MovieTable.ToList(); // list must be of MoiveModel types deined in model class.


           
            
            //showing 4 movies on each page.
            const int pageSize = 4;

            // checks if the page number (pg) is less than 1. If it is, we set it to 1 because there is no page 0 or negative page.
            if (currentPg < 1)
            {
                currentPg = 1;
            }

            // Calculate the total number of items/movies in DB
            int totalCount = data1.Count();

            // creating this "pagination" obj, we tell it how many movies we have, what page we're on, and how many movies should be on each page and it to
            // Pagination constructor in model class.
            var pagination = new Pagination(totalCount, currentPg, pageSize);

            // if we're on page 2, we skip the first 4 movies to start showing from the 5th movie
            int skip = (currentPg - 1) * pageSize;

            //data1.Skip(skip) skips Movie A, B, C, D. from totalcollections
            // After skipping the movies, the Take(pagination.PageSize) part tells the code to take the next PageSize number of movies.here it is 4 and
            // takes the next 4 movies: Movie E, F, G, H..
            var data = data1.Skip(skip).Take(pagination.PageSize).ToList();

            List<responseModel> responseList = new List<responseModel>();
            foreach(var item in data)
            {

                var viewModel = new responseModel
                {
                    Id = item.Id,
                    MovieName = item.MovieName,
                    Description = item.Description,
                    Director = item.Director,
                    Duration = item.Duration,
                    Genre = item.Genre,
                    Rating = item.Rating,
                   Image = item.ImageUrl,
                };
                responseList.Add(viewModel);

            }


          

            // Pass the pagination object to the view so that pagination info will send to view for display
            this.ViewBag.Pagination = pagination;

            // Return the view with the paginated data
            return View(responseList);
        }


        // GET: Movie/Details/5
        public async Task<IActionResult> Details(int? id) //  movie description
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
        [ValidateAntiForgeryToken] // only used for post methods
        public async Task<IActionResult> Create(MovieViewModel ViewModel)
        {

            if (ModelState.IsValid)
            {
                string moviePath = string.Empty;
                // Handle image upload if an image file is provided
                if (ViewModel?.Image != null && ViewModel.Image.Length > 0)
                {
                    var uploadResult = await UploadImageToCloudinary(ViewModel.Image);
                    if (uploadResult != null)
                    {
                        moviePath = uploadResult.Url.ToString();
                        
                    }
                }     

                // Check if the movie name already exists in the database(x.MovieName=database table data)
                var existingMovie = await _context.MovieTable.Where(x => x.MovieName == ViewModel.MovieName).SingleOrDefaultAsync();// FirstOrDefaultAsync=linq given by EF core

                if (existingMovie != null )
                {
                    // Movie name already exists, return a message
                    ViewBag.msg = "Cannot insert the same movie name twice.";
                    var genre = await _context.GenreTable.ToListAsync();

                    ViewBag.Genres = new SelectList(genre, "GenreId", "Name");
                    return View(ViewModel);

                }
                else {
                    var movieModel1 = new MovieModel
                    {
                        MovieName = ViewModel.MovieName,
                        Description = ViewModel.Description,
                        Director = ViewModel.Director,
                        Duration = ViewModel.Duration,
                        Genre = ViewModel.Genre,
                        Rating = ViewModel.Rating,
                        ImageUrl = moviePath
                    };

                    
                    _context.MovieTable.Add(movieModel1);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = "Movie added successfully!";
                    return RedirectToAction("List");
                }
            }

            return View();
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
            vm.Genre = movieModel.Genre;
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

                ViewBag.Genres = new SelectList(genre, "GenreId", "Name");
               
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
