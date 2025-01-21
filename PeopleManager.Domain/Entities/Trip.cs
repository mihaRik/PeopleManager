namespace PeopleManager.Domain.Entities;

public class Trip
{
    public int TripId { get; set; }
    public Guid ShareId { get; set; }
    public string Name { get; set; }
    public float Budget { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public IEnumerable<PlanItem> PlanItems { get; set; }
}