using Test.Task.Application.DTOs;
using Test.Task.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace DogshouseService.Api.Controllers;

[ApiController]
[Route("")]
public class DogsController : ControllerBase
{
    private readonly GetDogsUseCase _getDogs;
    private readonly CreateDogUseCase _createDog;

    public DogsController(GetDogsUseCase getDogs, CreateDogUseCase createDog)
    {
        _getDogs = getDogs;
        _createDog = createDog;
    }

    [HttpGet("ping")]
    public IActionResult PingTheVersion()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }

    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogsAsync(
        [FromQuery] string? attribute,
        [FromQuery] string? order,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await _getDogs.ExecuteAsync(
            attribute, order, pageNumber ?? 1, pageSize ?? 100, cancellationToken);

        return Ok(result);
    }

    [HttpPost("dogs")]
    public async Task<IActionResult> CreateDogAsync([FromBody] DogDto dto, CancellationToken cancellationToken)
    {
        await _createDog.ExecuteAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetDogsAsync), null);
    }
}
