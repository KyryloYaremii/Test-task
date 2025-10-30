using Test.Task.Api.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Test.Task.Infrastructure.Data;
using Test.Task.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<DogDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dogshouse Service API",
        Version = "1.0.1",
        Description = "API for managing dogs."
    });
});
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<DogDbContext>();

        //Uncommit if you need to re-create db
        /*dbContext.Database.EnsureDeleted();*/

        //I seeded db with some data as i can see in example
        //Will use migration, to have opportunity to clear data in db every time app starts and to have an easy access
        //to seed data because there is no delete method
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the DB.");
    }
}

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionHandlerPathFeature?.Error != null)
        {
            var exception = exceptionHandlerPathFeature.Error;

            switch (exception)
            {
                case DuplicateDogNameException ex:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await Results.Problem(
                        title: "Dog name conflict",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status409Conflict
                    ).ExecuteAsync(context);
                    break;


                 case DogNotFoundException ex:
                     context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await Results.Problem(
                        title: "A dog with such parameters does not exist.",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status404NotFound
                        ).ExecuteAsync(context);
                     break;


                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await Results.Problem(
                        title: "An unexpected error occurred.",
                        detail: "Internal Server Error. Please try again later.",
                        statusCode: StatusCodes.Status500InternalServerError
                    ).ExecuteAsync(context);
                    break;
            }
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();