namespace Test1Retake.Services.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services.AddScoped<ITeamMemberService, TeamMemberService>();
        // services.AddScoped<ITaskService, TaskService>();
        
        return services;
    }
}