using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: Program <ClassName> <MethodName>");
            return;
        }

        string className = args[0];
        string methodName = args[1];

        Type type = Type.GetType(className);
        if (type == null)
        {
            Console.WriteLine($"Class {className} not found.");
            return;
        }

        object instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            Console.WriteLine($"Failed to create instance of {className}.");
            return;
        }

        var methodInfo = type.GetMethod(methodName);
        if (methodInfo == null)
        {
            Console.WriteLine($"Method {methodName} not found in class {className}.");
            return;
        }

        methodInfo.Invoke(instance, null);
    }
}
