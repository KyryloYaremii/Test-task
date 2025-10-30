using Test.Task.Application.DTOs;
using Test.Task.Application.Exceptions;
using Test.Task.Application.Interfaces;
using Test.Task.Domain.Entities;

namespace Test.Task.Application.UseCases;

public class CreateDogUseCase
{
    private readonly IDogRepository _repository;

    public CreateDogUseCase(IDogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Dog> ExecuteAsync(DogDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Dog name is required.", nameof(dto.Name));

        if (await _repository.DogNameExistsAsync(dto.Name, cancellationToken))
        {
            throw new DuplicateDogNameException($"A dog with the name '{dto.Name}' already exists.");
        }

        var dog = new Dog
        {
            Name = dto.Name,
            Color = dto.Color,
            TailLength = dto.TailLength,
            Weight = dto.Weight
        };

        await _repository.AddDogAsync(dog, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return dog;
    }
}
