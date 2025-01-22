using Microsoft.Extensions.DependencyInjection;
using PeopleManager.Logic.Helpers;
using PeopleManager.Repository.Helpers;
using PeopleManager.Screens;

var services = new ServiceCollection();

services
    .AddScreens()
    .AddLogicServices()
    .AddRepositories();

var serviceProvider = services.BuildServiceProvider();
var app = serviceProvider.GetRequiredService<MainScreen>();

await app.StartAsync();