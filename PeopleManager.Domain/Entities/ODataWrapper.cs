namespace PeopleManager.Domain.Entities;

public class ODataWrapper<T>
{
    public IEnumerable<T> Value { get; set; }
}