using PeopleManager.Domain.Entities;

namespace PeopleManager.Repository;

public interface IPeopleRepository : IReadOnlyPeopleRepository
{
    Task<Person> UpdatePersonAsync(string username, Person person, CancellationToken cancellationToken);
}