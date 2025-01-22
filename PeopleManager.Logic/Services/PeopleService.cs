using System.Reflection;
using PeopleManager.Domain.Entities;
using PeopleManager.Logic.Helpers;
using PeopleManager.Repository;

namespace PeopleManager.Logic.Services;

public class PeopleService : IPeopleService
{
    private readonly IPeopleRepository _peopleRepository;

    public PeopleService(IPeopleRepository peopleRepository)
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

    public async Task<Person> UpdatePersonAsync(
        Person person,
        PropertyInfo propertyToUpdate,
        object newValue,
        CancellationToken cancellationToken)
    {
        var username = person.UserName;
        propertyToUpdate.SetValue(person, newValue);
        
        return await _peopleRepository.UpdatePersonAsync(username, person, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _peopleRepository.Dispose();
        }
    }

    ~PeopleService()
    {
        Dispose(false);
    }
}