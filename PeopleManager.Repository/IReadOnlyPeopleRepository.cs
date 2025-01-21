using PeopleManager.Domain.Entities;

namespace PeopleManager.Repository;

public interface IReadOnlyPeopleRepository
{
    Task<IEnumerable<Person>> GetPeopleAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<int> GetPeopleCountAsync(string searchQuery = null, CancellationToken cancellationToken = default);
    
    Task<Person> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken);
    
    Task<IEnumerable<Person>> SearchPeopleAsync(string searchQuery,int page, int pageSize, CancellationToken cancellationToken);
}