using PeopleManager.Domain.Entities;

namespace PeopleManager.Repository;

public interface IPeopleRepository : IReadOnlyPeopleRepository, IDisposable
{
    Task<Person> UpdatePersonAsync(string username, Person person, CancellationToken cancellationToken);
}