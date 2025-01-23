using Microsoft.Extensions.DependencyInjection;

namespace PeopleManager.Screens;

public static class ScreenRegistrationExtensions
{
    public static IServiceCollection AddScreens(this IServiceCollection services)
    {
        services.AddScoped<MainScreen>();
        services.AddScoped<OptionsScreen>();
        services.AddScoped<PersonScreen>();
        services.AddScoped<PersonUpdateScreen>();
        services.AddScoped<OptionsScreen>();
        services.AddScoped<PeopleScreen>();
        
        return services;
    }
}