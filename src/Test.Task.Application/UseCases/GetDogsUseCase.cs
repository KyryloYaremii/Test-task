using Test.Task.Application.DTOs;
using Test.Task.Application.Interfaces;

namespace Test.Task.Application.UseCases;

public class GetDogsUseCase
{
    private readonly IDogRepository _repository;
    private static readonly HashSet<string> AllowedSortFields = new()
        {
            "name", "color", "tail_length", "weight"
        };

    public GetDogsUseCase(IDogRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DogDto>> ExecuteAsync(
        string? attribute = null,
        string? order = null,
        int pageNumber = 1,
        int pageSize = 2,
        CancellationToken cancellationToken = default)
    {
        string? sortBy = null;
        bool desc = false;

        if (!string.IsNullOrWhiteSpace(attribute))
        {
            var normalized = attribute.Trim().ToLower();
            normalized = normalized switch
            {
                "tail_length" or "taillength" => "tail_length",
                "weight" => "weight",
                "color" => "color",
                "name" => "name",
                _ => normalized
            };

            if (AllowedSortFields.Contains(normalized))
                sortBy = normalized;
        }

        if (!string.IsNullOrWhiteSpace(order))
            desc = order.Trim().ToLower() == "desc";

        var dogs = await _repository.GetAllAsync(sortBy, desc, pageNumber, pageSize, cancellationToken);

        return dogs.Select(d => new DogDto
        {
            Name = d.Name,
            Color = d.Color,
            TailLength = d.TailLength,
            Weight = d.Weight
        }).ToList();
    }
}

