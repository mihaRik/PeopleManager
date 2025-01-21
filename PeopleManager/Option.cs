namespace PeopleManager;

public class Option
{
    public string Name { get; set; }
    public Func<object[], CancellationToken, Task> Action { get; set; }

    public object[] ActionParams { get; set; }
}