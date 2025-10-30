using FluentAssertions;
using Moq; 
using System.Net.NetworkInformation;
using Test.Task.Application.DTOs;
using Test.Task.Application.Exceptions;
using Test.Task.Application.Interfaces;
using Test.Task.Application.UseCases;
using Test.Task.Domain.Entities;

namespace Test.Task.Application.Tests;
public class GetDogByIdUseCaseTests
{
    private readonly Mock<IDogRepository> _mockRepo;
    private readonly GetDogByIdUseCase _useCase;

    public GetDogByIdUseCaseTests()
    {
        _mockRepo = new Mock<IDogRepository>();
        _useCase = new GetDogByIdUseCase(_mockRepo.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldReturnDogDto_WhenDogExists()
    {
        var dogId = 5;
        var fakeDogFromDb = new Dog
        {
            Id = dogId,
            Name = "Neo",
            Color = "red",
            Weight = 10,
            TailLength = 5
        };

        _mockRepo.Setup(r => r.GetDogByIdAsync(dogId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(fakeDogFromDb);

        var result = await _useCase.ExecuteAsync(dogId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<DogDto>();

        result.Name.Should().Be("Neo");
        result.Weight.Should().Be(10);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldThrowDogNotFoundException_WhenDogDoesNotExist()
    {
        var dogId = 9999; 

        _mockRepo.Setup(r => r.GetDogByIdAsync(dogId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Dog)null);

        Func<System.Threading.Tasks.Task> act = async () => await _useCase.ExecuteAsync(dogId, CancellationToken.None);

        await act.Should().ThrowAsync<DogNotFoundException>()
                 .WithMessage($"Dog with Id {dogId} not found.");
    }
}