using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using MovieApplication.Models;

namespace MovieApplication.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly MovieContext _context;

        public MovieController(MovieContext context)
        {
            _context = context;
        }

        // GET: MovieList
        public IActionResult List(int currentPg = 1) // if the parameter is not provided when the method is called, it will default to 1.
        {
            // Retrieve the list of movies from the database
            List<MovieModel> data1 = _context.MovieTable.ToList(); // list must be of MoiveModel types deined in model class.

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

            // Pass the pagination object to the view so that pagination info will send to view for display
            this.ViewBag.Pagination = pagination;

            // Return the view with the paginated data
            return View(data);
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
            ViewBag.Genres = new SelectList(genreList, "GenreId", "Name");


            return View();
        }
        //Create
        [HttpPost]
        [ValidateAntiForgeryToken] // only used for post methods
        public async Task<IActionResult> Create(MovieModel movieModel)
        {
            if (ModelState.IsValid)
            {
                // Check if the movie name already exists in the database(x.MovieName=database table data)
                var existingMovie = await _context.MovieTable.Where(x => x.MovieName == movieModel.MovieName).ToListAsync();// FirstOrDefaultAsync=linq given by EF core

                if (existingMovie != null && existingMovie.Count()>0)
                {
                    // Movie name already exists, return a message
                    ViewBag.msg = "Cannot insert the same movie name twice.";
                    var genre = await _context.GenreTable.ToListAsync();

                    ViewBag.Genres = new SelectList(genre, "GenreId", "Name");
                    return View(movieModel);
                }
                else
                {

                    _context.MovieTable.Add(movieModel);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = "Movie added successfully!";
                    return RedirectToAction("List");
                }
            }
            var genreList = await _context.GenreTable.ToListAsync();
            
            ViewBag.Genres = new SelectList(genreList, "GenreId", "Name");
            return View(movieModel);
        }


        // GET: Movie/Edit/5
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieModel = await _context.MovieTable.FindAsync(id);
            if (movieModel == null)
            {
                return NotFound();
            }
            return View(movieModel);
        }

        // POST: Movie/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(MovieModel movieModel)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.MovieTable.Update(movieModel);
                    await _context.SaveChangesAsync();
                    ViewBag.msg = "Updated Successfully!";
                 

                }
                catch (DbUpdateConcurrencyException) // 2 users try to update db at same time this excpt. occurs
                {
                    if (!MovieModelExists(movieModel.Id)) // if the exception is not due to the movie model not being found, it throw to higher level excpt hadling.
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
            return View(movieModel);
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
