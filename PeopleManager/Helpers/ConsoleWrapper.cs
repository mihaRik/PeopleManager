namespace PeopleManager.Helpers;

public class ConsoleWrapper : IConsoleWrapper
{
    public ConsoleKeyInfo ReadKey()
    {
        return Console.ReadKey();
    }
}