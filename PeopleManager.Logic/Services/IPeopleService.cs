using System.Reflection;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;

namespace PeopleManager.Logic.Services;

public interface IPeopleService
{
    Task<PagedResult<Person>> GetPeopleAsync(int page, int pageSize, CancellationToken cancellationToken);
    
    Task<Person> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<PagedResult<Person>> SearchPeopleAsync(string searchQuery, int page, int pageSize, CancellationToken cancellationToken);
    
    Task<Person> UpdatePersonAsync(
        Person person,
        PropertyInfo propertyToUpdate,
        object newValue,
        CancellationToken cancellationToken);
    
    Task<Person> CreatePersonAsync(Person person, CancellationToken cancellationToken);
}