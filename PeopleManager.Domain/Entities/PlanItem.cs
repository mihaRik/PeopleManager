namespace PeopleManager.Domain.Entities;

public class PlanItem
{
    public int PlanItemId { get; set; }
    public string ConfirmationCode { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public TimeSpan Duration { get; set; }
}