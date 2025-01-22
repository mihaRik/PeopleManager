namespace PeopleManager;

#nullable disable

public class Option
{
    public string Name { get; init; }
    public Func<object[], CancellationToken, Task> Action { get; init; }

    public object[] ActionParams { get; init; }
}