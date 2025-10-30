namespace Test.Task.Application.Exceptions;

public class DogNotFoundException : Exception
{
    public DogNotFoundException(string message) : base(message) { }

}