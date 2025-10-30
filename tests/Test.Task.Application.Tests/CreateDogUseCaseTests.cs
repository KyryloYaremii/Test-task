using Moq; 
using FluentAssertions;
using Test.Task.Application.Interfaces;
using Test.Task.Application.UseCases;
using Test.Task.Application.DTOs;
using Test.Task.Domain.Entities;
using Test.Task.Application.Exceptions;

namespace Test.Task.Application.Tests;

public class CreateDogUseCaseTests
{
    private readonly Mock<IDogRepository> _mockRepo;
    private readonly CreateDogUseCase _useCase;

    public CreateDogUseCaseTests()
    {
        _mockRepo = new Mock<IDogRepository>();
        _useCase = new CreateDogUseCase(_mockRepo.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldCreateDog_WhenNameIsUnique()
    {

        var dogDto = new DogDto
        {
            Name = "Neo",
            Color = "red",
            TailLength = 10,
            Weight = 5
        };

        _mockRepo.Setup(r => r.DogNameExistsAsync("Neo", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

        var result = await _useCase.ExecuteAsync(dogDto, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Neo");
        result.Weight.Should().Be(5);
        result.Should().BeOfType<Dog>();

        _mockRepo.Verify(r => r.AddDogAsync(
            It.IsAny<Dog>(),
            It.IsAny<CancellationToken>()),
            Times.Once);

        _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldThrowDuplicateDogNameException_WhenNameExists()
    {
        var dogDto = new DogDto
        {
            Name = "Neo", 
            Color = "red",
            TailLength = 10,
            Weight = 5
        };

        _mockRepo.Setup(r => r.DogNameExistsAsync("Neo", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true); 

        Func<System.Threading.Tasks.Task> act = async () => await _useCase.ExecuteAsync(dogDto, CancellationToken.None);

        await act.Should().ThrowAsync<DuplicateDogNameException>()
                 .WithMessage($"A dog with the name '{dogDto.Name}' already exists.");

        _mockRepo.Verify(r => r.AddDogAsync(
            It.IsAny<Dog>(),
            It.IsAny<CancellationToken>()),
            Times.Never); 

        _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldThrowArgumentNullException_WhenDtoIsNull()
    {
     
        DogDto dogDto = null; 

        Func<System.Threading.Tasks.Task> act = async () => await _useCase.ExecuteAsync(dogDto, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();

        _mockRepo.Verify(r => r.DogNameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockRepo.Verify(r => r.AddDogAsync(It.IsAny<Dog>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(null)] // Name = null
    [InlineData("")] // Name = ""
    [InlineData("   ")] // Name = "   "
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace(string invalidName)
    {
        var dogDto = new DogDto
        {
            Name = invalidName,
            Color = "red",
            TailLength = 10,
            Weight = 5
        };

        Func<System.Threading.Tasks.Task> act = async () => await _useCase.ExecuteAsync(dogDto, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();

        _mockRepo.Verify(r => r.DogNameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}