namespace PeopleManager.Domain.Entities;

public class Flight : PublicTransportation
{
    public string FlightNumber { get; set; }
    public Airline Airline { get; set; }
    public Airport From { get; set; }
    public Airport To { get; set; }
}