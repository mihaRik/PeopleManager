using PeopleManager.Domain.Models;

namespace PeopleManager.Domain.Entities;

public class Manager : Person
{
    public long Budget { get; set; }
    public Location BossOffice { get; set; }
    public IEnumerable<Person> DirectReports { get; set; }
}