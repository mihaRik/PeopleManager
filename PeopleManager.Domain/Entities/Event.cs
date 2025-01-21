using PeopleManager.Domain.Models;

namespace PeopleManager.Domain.Entities;

public class Event : PlanItem
{
    public EventLocation OccursAt { get; set; }
    public string Description { get; set; }
}