using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Data;
using MovieApplication.Models;

namespace MovieApplication.Controllers
{
    public class GenreController : Controller
    {
        private readonly MovieContext _context;

        public GenreController(MovieContext context)
        {
            _context = context;
        }

        //Get: genre/list
        // Task is a type representing an asynchronous operation that produces a result of type void, i.e., it doesn't return a value. Without Task, you wouldn't be able to use await inside the method.
        public async Task<IActionResult> Index()//IActionResult is an interface that represents the result or returntype of an action method.
        {
          var data = await _context.GenreTable.ToListAsync();
            return View(data);

        }

        //Get:Genre/Details/id=5
        public async Task<IActionResult> Details(int ? id) // id can be int or null
        {
            if(id == null)
            {
                return NotFound();
            }

            var genre = await _context.GenreTable.FirstOrDefaultAsync(m=>m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        //Get:create
        public IActionResult Create() {
            return View();

                }
        //Post:create
        [HttpPost]
    public async Task<IActionResult> Create(GenreModel genre) {
            if (ModelState.IsValid)
            {
                _context.GenreTable.Add(genre);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        //Get:Edit/id
        public async Task<IActionResult> Edit(int ? id) {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.GenreTable.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        //Post:Genre/Edit/Id=5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, GenreModel genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    _context.GenreTable.Update(genre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();
                    
                }
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        //get:delete/5
        public async Task<IActionResult> Delete(int ? id)
        {
            if (id == null) { 
            return NotFound();  
            }
            var genre = await _context.GenreTable.FirstOrDefaultAsync(m => m.GenreId == id);
             
            if(genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        //post:genre/delete/id
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var genre =await _context.GenreTable.FindAsync(id);

            if (genre != null)
            {
                _context.GenreTable.Remove(genre);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    
    
    }
}
