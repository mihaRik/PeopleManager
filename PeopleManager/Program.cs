using Microsoft.Extensions.DependencyInjection;
using PeopleManager;
using PeopleManager.Logic.Services;
using PeopleManager.Repository;
using PeopleManager.Repository.Client;

var serviceCollection = new ServiceCollection();
serviceCollection.AddHttpClient<IODataHttpClient, ODataHttpClient>(config =>
{
    config.BaseAddress = new Uri("http://services.odata.org/TripPinRESTierService/(S(ibiqexd5sgywtjtvpesut30t))");
});
serviceCollection.AddScoped<IPeopleService, PeopleService>();
serviceCollection.AddScoped<IReadOnlyPeopleRepository, PeopleRepository>();
serviceCollection.AddScoped<OptionSelector>();

var serviceProvider = serviceCollection.BuildServiceProvider();
var optionSelector = serviceProvider.GetRequiredService<OptionSelector>();

await optionSelector.StartAsync();