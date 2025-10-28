using Test.Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Test.Task.Infrastructure.Data
{
    public class DogDbContext : DbContext
    {
        public DbSet<Dog> Dogs { get; set; } = null!;

        public DogDbContext(DbContextOptions<DogDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DogDbContext).Assembly);
        }
    }
}
