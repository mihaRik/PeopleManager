using PeopleManager.Domain.Enums;
using PeopleManager.Domain.Models;

namespace PeopleManager.Domain.Entities;

public class Person
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public PersonGender Gender { get; set; }
    public long? Age { get; set; }
    public IEnumerable<string> Emails { get; set; }
    public IEnumerable<Location> AddressInfo { get; set; }
    public Location HomeAddress { get; set; }
    public Feature FavoriteFeature { get; set; }
    public IEnumerable<Feature> Features { get; set; }
    public IEnumerable<Person> Friends { get; set; }
    public Person BestFriend { get; set; }
    public IEnumerable<Trip> Trips { get; set; }
}