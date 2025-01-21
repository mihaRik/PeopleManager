using PeopleManager.Domain.Models;

namespace PeopleManager.Domain.Entities;

public class Airport
{
    public string Name { get; set; }
    public string IcaoCode { get; set; }
    public string IataCode { get; set; }
    public AirportLocation Location { get; set; }
}