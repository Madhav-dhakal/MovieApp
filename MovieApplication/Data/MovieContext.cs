using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApplication.Models;
using MovieApplication.ViewModel;
namespace MovieApplication.Data
{
    public class MovieContext : IdentityDbContext
    {

        public MovieContext(DbContextOptions<MovieContext> options) : base(options)
        {

        }
        public DbSet<MovieModel> MovieTable { get; set; }
        public DbSet<GenreModel> GenreTable { get; set; }
        public DbSet<ReviewModel> Reviews { get; set; }
        public DbSet<Ratings> Ratings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MovieModel>()
                .HasMany(m => m.Reviews)
                .WithOne(r => r.Movie)
                .HasForeignKey(r => r.MovieId);

            /*modelBuilder.Entity<MovieModel>()
                .HasMany(m => m.Ratings)
                .WithOne(r => r.Movie)
                .HasForeignKey(r => r.MovieId);*/
        }
    }
}
