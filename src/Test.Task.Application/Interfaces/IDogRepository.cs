using Test.Task.Domain.Entities;

namespace Test.Task.Application.Interfaces;

public interface IDogRepository
{
    Task<List<Dog>> GetAllAsync(
        string? sortBy = null, bool desc = false,
        int? pageNumber = null, int? pageSize = null,
        CancellationToken cancellationToken = default);

    Task<Dog> AddDogAsync(Dog dog, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> DogNameExistsAsync(string name, CancellationToken cancellationToken = default);
}