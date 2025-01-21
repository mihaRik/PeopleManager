using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;

namespace PeopleManager.Logic.Services;

public interface IPeopleService
{
    Task<PagedResult<Person>> GetPeopleAsync(int page, int pageSize, CancellationToken cancellationToken);
    
    Task<Person> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<PagedResult<Person>> SearchPeopleAsync(string searchQuery, int page, int pageSize, CancellationToken cancellationToken);
}