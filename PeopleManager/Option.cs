namespace PeopleManager;

#nullable disable

public class Option
{
    public string Name { get; init; }
    public Func<object[], CancellationToken, Task> Action { get; init; }

    public object[] ActionParams { get; init; }

    public Option Clone()
    {
        var copyArray = new object[ActionParams.Length];
        ActionParams.CopyTo(copyArray, 0);
        return new Option
        {
            Name = Name,
            Action = Action,
            ActionParams = copyArray,
        };
    }
}