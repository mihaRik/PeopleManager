using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;
using PeopleManager.Repository;

namespace PeopleManager.Logic.Services;

public class PeopleService : IPeopleService
{
    private readonly IReadOnlyPeopleRepository _peopleRepository;

    public PeopleService(IReadOnlyPeopleRepository peopleRepository)
    {
        _peopleRepository = peopleRepository;
    }

    public async Task<PagedResult<Person>> GetPeopleAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var people = await _peopleRepository.GetPeopleAsync(page, pageSize, cancellationToken).ConfigureAwait(false);
        var totalCount = await _peopleRepository.GetPeopleCountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return new PagedResult<Person>
        {
            Items = people,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<Person> GetPersonByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var person = await _peopleRepository.GetPersonByUsernameAsync(username, cancellationToken).ConfigureAwait(false);

        if (person == null)
        {
            throw new KeyNotFoundException($"Person with username '{username}' not found.");
        }

        return person;
    }

    public async Task<PagedResult<Person>> SearchPeopleAsync(string searchQuery, int page, int pageSize, CancellationToken cancellationToken)
    {
        var people = await _peopleRepository.SearchPeopleAsync(searchQuery, page, pageSize, cancellationToken).ConfigureAwait(false);
        var totalCount = await _peopleRepository.GetPeopleCountAsync(searchQuery, cancellationToken).ConfigureAwait(false);
        
        return new PagedResult<Person>
        {
            Items = people,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }
}