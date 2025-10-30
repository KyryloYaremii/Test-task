using System.ComponentModel.DataAnnotations;

namespace Test.Task.Application.DTOs;

public class DogDto
{
    [Required(ErrorMessage = "Dog name is required.")]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Color is required.")]
    public required string Color { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Tail length cannot be a negative number.")]
    public int TailLength { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Weight must be a positive number.")]
    public int Weight { get; set; }
}