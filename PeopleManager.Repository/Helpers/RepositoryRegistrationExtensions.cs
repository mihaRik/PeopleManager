using Microsoft.Extensions.DependencyInjection;
using PeopleManager.Repository.Client;

namespace PeopleManager.Repository.Helpers;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IReadOnlyPeopleRepository, PeopleRepository>();
        services.AddScoped<IPeopleRepository, PeopleRepository>();
        
        services.AddHttpClient<IODataHttpClient, ODataHttpClient>(config =>
        {
            config.BaseAddress = new Uri("http://services.odata.org/TripPinRESTierService/(S(ibiqexd5sgywtjtvpesut30t))");
        });
        
        return services;
    }
}