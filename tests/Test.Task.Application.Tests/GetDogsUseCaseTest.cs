using Moq;
using FluentAssertions;
using Test.Task.Application.Interfaces;
using Test.Task.Application.UseCases;
using Test.Task.Application.DTOs;
using Test.Task.Domain.Entities;

namespace Test.Task.Application.Tests;

public class GetDogsUseCaseTests
{
    private readonly Mock<IDogRepository> _mockRepo;
    private readonly GetDogsUseCase _useCase;

    public GetDogsUseCaseTests()
    {
        _mockRepo = new Mock<IDogRepository>();
        _useCase = new GetDogsUseCase(_mockRepo.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldReturnMappedDogDtos_WhenDogsExist()
    {

        var fakeDogsFromDb = new List<Dog>
        {
            new Dog { Id = 1, Name = "Neo", Color = "red", Weight = 10, TailLength = 5 },
            new Dog { Id = 2, Name = "Jessy", Color = "black", Weight = 7, TailLength = 3 }
        };

        _mockRepo.Setup(r => r.GetAllAsync(
            It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
                 .ReturnsAsync(fakeDogsFromDb);

        var result = await _useCase.ExecuteAsync(null, null, 1, 10, CancellationToken.None);

        result.Should().BeOfType<List<DogDto>>();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Neo");
        result[1].Name.Should().Be("Jessy");
    }
    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldCallRepository_WithCorrectSortParameters()
    {
        string attribute = "weight";
        string order = "desc";

        _mockRepo.Setup(r => r.GetAllAsync(
            It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<Dog>());

        await _useCase.ExecuteAsync(attribute, order, 1, 10, CancellationToken.None);

        _mockRepo.Verify(r => r.GetAllAsync(
            "weight",
            true,
            1,
            10,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldCallRepository_WithNullSortBy_WhenAttributeIsInvalid()
    {
        string attribute = "some_invalid_field";

        _mockRepo.Setup(r => r.GetAllAsync(
            It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new List<Dog>());

     
        await _useCase.ExecuteAsync(attribute, "asc", 1, 10, CancellationToken.None); 

        _mockRepo.Verify(r => r.GetAllAsync(
            null,    
            false,
            1,
            10,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]  // pageNumber = 0, pgsize = 10
    [InlineData(-1, 10)] // pageNumber = -1, pgsize = 10
    [InlineData(1, 0)]   // pageSize = 0, pgsize = 0
    [InlineData(1, -5)]  // pageSize = -5, pgsize = -5
    public async System.Threading.Tasks.Task ExecuteAsync_ShouldThrowArgumentException_WhenPaginationIsInvalid(int pageNumber, int pageSize)
    {
        Func<System.Threading.Tasks.Task> act = async () => await _useCase.ExecuteAsync(
            null,
            null,
            pageNumber,
            pageSize,
            CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();

        _mockRepo.Verify(r => r.GetAllAsync(
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
}