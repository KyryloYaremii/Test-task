using Test.Task.Application.Interfaces;
using Test.Task.Domain.Entities;
using Test.Task.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Test.Task.Infrastructure.Repositories;

public class DogRepository : IDogRepository
{
    private readonly DogDbContext _context;

    public DogRepository(DogDbContext context)
    {
        _context = context;
    }

    public async Task<Dog> AddDogAsync(Dog dog, CancellationToken cancellationToken = default)
    {
        await _context.Dogs.AddAsync(dog, cancellationToken);
        return dog;
    }

    public async Task<List<Dog>> GetAllAsync(
        string? sortBy = null, bool desc = false,
        int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Dog> query = _context.Dogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy switch
            {
                "name" => desc ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "color" => desc ? query.OrderByDescending(d => d.Color) : query.OrderBy(d => d.Color),
                "tail_length" => desc ? query.OrderByDescending(d => d.TailLength) : query.OrderBy(d => d.TailLength),
                "weight" => desc ? query.OrderByDescending(d => d.Weight) : query.OrderBy(d => d.Weight),
                _ => query
            };
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            var skip = (pageNumber.Value - 1) * pageSize.Value;
            query = query.Skip(skip).Take(pageSize.Value);
        }
        else if (pageSize.HasValue)
        {
            query = query.Take(pageSize.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DogNameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Dogs.AnyAsync(d => d.Name == name, cancellationToken);
    }
}
