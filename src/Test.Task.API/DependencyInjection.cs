using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using Test.Task.Application.Interfaces;
using Test.Task.Infrastructure.Repositories;
using Test.Task.Application.UseCases;

namespace Test.Task.Api.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDogRepository, DogRepository>();
        services.AddScoped<GetDogsUseCase>();
        services.AddScoped<CreateDogUseCase>();
        return services;
    }
}