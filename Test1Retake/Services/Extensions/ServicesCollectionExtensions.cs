using Test1Retake.Services.Abstractions;

namespace Test1Retake.Services.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();
        
        return services;
    }
}