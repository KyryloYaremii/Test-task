namespace Test.Task.Application.Exceptions;
public class DuplicateDogNameException : Exception
{
    public DuplicateDogNameException(string message) : base(message) { }
}