using Test.Task.Application.DTOs;
using Test.Task.Application.Exceptions;
using Test.Task.Application.Interfaces;
using Test.Task.Domain.Entities;

namespace Test.Task.Application.UseCases;
public class GetDogByIdUseCase
{
    private readonly IDogRepository _repository;

    public GetDogByIdUseCase(IDogRepository repository)
    {
        _repository = repository;
    }

    public async Task<DogDto> ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        var dog = await _repository.GetDogByIdAsync(id, cancellationToken);

        if (dog is null)
        {
            throw new DogNotFoundException($"Dog with Id {id} not found.");
        }

        return new DogDto
        {
            Name = dog.Name,
            Color = dog.Color,
            TailLength = dog.TailLength,
            Weight = dog.Weight
        };
    }
}