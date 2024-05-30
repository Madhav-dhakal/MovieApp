using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Models;
using MovieApplication.ViewModel;
namespace MovieApplication.Data
{
    public class MovieContext : IdentityDbContext
    {
        public MovieContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<MovieModel> MovieTable { get; set; }
        public DbSet<GenreModel> GenreTable { get; set; }
    }
}
