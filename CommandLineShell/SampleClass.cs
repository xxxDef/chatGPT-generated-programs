using System;

public class SampleClass
{
    public void MethodWithoutParameters()
    {
        Console.WriteLine("MethodWithoutParameters called");
    }

    public void MethodWithOneParameter(string str)
    {
        Console.WriteLine($"MethodWithOneParameter called with parameter: {str}");
    }

    public void MethodWithMultipleParameters(string str, int num, DateTime date)
    {
        Console.WriteLine($"MethodWithMultipleParameters called with parameters: {str}, {num}, {date}");
    }
}
