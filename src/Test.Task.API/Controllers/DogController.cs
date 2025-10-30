using Microsoft.AspNetCore.Mvc;
using System.Runtime.ExceptionServices;
using Test.Task.Application.DTOs;
using Test.Task.Application.UseCases;
using Test.Task.Domain.Entities;

namespace DogshouseService.Api.Controllers;

[ApiController]
[Route("")]
public class DogsController : ControllerBase
{
    private readonly GetDogsUseCase _getDogs;
    private readonly CreateDogUseCase _createDog;
    private readonly GetDogByIdUseCase _getDogById;

    public DogsController(GetDogsUseCase getDogs, CreateDogUseCase createDog, GetDogByIdUseCase detDogById)
    {
        _getDogs = getDogs;
        _createDog = createDog;
        _getDogById = detDogById;
    }

    /// <summary>
    /// Get the api version.
    /// </summary>
    /// <returns>Returns api version.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("ping")]
    public IActionResult PingTheVersion()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }

    /// <summary>
    /// Get all dogs with sorting and pagination.
    /// </summary>
    /// <param name="attribute">Parameter to sort by ('name', 'weight').</param>
    /// <param name="order">Sort order ('asc' or 'desc').</param>
    /// <param name="pageNumber">Number of page (must be > 0).</param>
    /// <param name="pageSize">Page size (must be > 0).</param>
    /// <param name="cancellationToken">Token to cancel operation.</param>
    /// <returns>A list with dogs.</returns>
    [ProducesResponseType(typeof(List<DogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogsAsync(
        [FromQuery] string? attribute,
        [FromQuery] string? order,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        // Эта логика (?? 1) здесь абсолютно правильная
        var result = await _getDogs.ExecuteAsync(
            attribute, order, pageNumber ?? 1, pageSize ?? 100, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new dog.
    /// </summary>
    /// <param name="dto">The dog to create.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly created dog.</returns>

    [ProducesResponseType(typeof(Dog), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("dogs")]
    public async Task<IActionResult> CreateDogAsync([FromBody] DogDto dto, CancellationToken cancellationToken)
    {
        var newDog = await _createDog.ExecuteAsync(dto, cancellationToken);
        return CreatedAtRoute(
        nameof(GetDogByIdAsync),
        new { id = newDog.Id },
        newDog);
    }

    /// <summary>
    /// Gets a specific dog by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the dog to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel operation.</param>
    /// <returns>The dog details.</returns>
    [HttpGet("dogs/{id}", Name = "GetDogByIdAsync")]
    [ProducesResponseType(typeof(DogDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDogByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dogDto = await _getDogById.ExecuteAsync(id, cancellationToken);

        return Ok(dogDto);
    }
}
