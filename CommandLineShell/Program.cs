using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

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

    static void PrintHelp()
    {
        Console.WriteLine("Usage: Program <ClassName> <MethodName>");
        Console.WriteLine("Possible classes:");
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsPublic)
            {
                Console.WriteLine($"- {type.Name}");
                Console.WriteLine("  Possible methods:");
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    Console.WriteLine($"  - {method.Name}");
                }
            }
        }
    }
}
