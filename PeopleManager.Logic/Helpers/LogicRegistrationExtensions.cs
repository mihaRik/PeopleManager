using Microsoft.Extensions.DependencyInjection;
using PeopleManager.Logic.Services;

namespace PeopleManager.Logic.Helpers;

public static class LogicRegistrationExtensions
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        services.AddScoped<IPeopleService, PeopleService>();
        
        return services;
    }
}