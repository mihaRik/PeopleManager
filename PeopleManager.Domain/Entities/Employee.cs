namespace PeopleManager.Domain.Entities;

public class Employee : Person
{
    public long Cost { get; set; }
    public IEnumerable<Person> Peers { get; set; }
}