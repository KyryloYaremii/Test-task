using Test.Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Test.Task.Infrastructure.Data;

public class DogDbContext : DbContext
{
    public DbSet<Dog> Dogs { get; set; } = null!;

    public DogDbContext(DbContextOptions<DogDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DogDbContext).Assembly);

        modelBuilder.Entity<Dog>().HasData(
    new Dog
    {
        Id = 1,
        Name = "Neo",
        Color = "red&amber",
        TailLength = 22,
        Weight = 32
    },
    new Dog
    {
        Id = 2,
        Name = "Jessy",
        Color = "black&white",
        TailLength = 7,
        Weight = 14
    });
    }
}
