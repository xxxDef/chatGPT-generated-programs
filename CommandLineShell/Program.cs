using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 1 && args[0].ToLower() == "interactive")
        {
            RunInteractiveMode();
        }
        else if (args.Length >= 2)
        {
            RunCommandMode(args);
        }
        else
        {
            PrintHelp();
        }
    }

    static void RunInteractiveMode()
    {
        var classes = GetPublicClasses();

        if (classes.Count == 0)
        {
            Console.WriteLine("No classes found.");
            return;
        }

        Console.WriteLine("Available classes:");
        for (int i = 0; i < classes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {classes[i].Name}");
        }

        int classIndex = ReadInt("Enter class number:");
        if (classIndex < 1 || classIndex > classes.Count)
        {
            Console.WriteLine("Invalid class number.");
            return;
        }

        Type type = classes[classIndex - 1];
        object instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            Console.WriteLine($"Failed to create instance of {type.Name}.");
            return;
        }

        var methods = GetPublicInstanceMethods(type);
        if (methods.Count == 0)
        {
            Console.WriteLine($"No methods found in class {type.Name}.");
            return;
        }

        Console.WriteLine($"Available methods in {type.Name}:");
        for (int i = 0; i < methods.Count; i++)
        {
            Console.Write($"{i + 1}. {methods[i].Name}(");
            Console.Write(string.Join(", ", methods[i].GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}")));
            Console.WriteLine(")");
        }

        int methodIndex = ReadInt("Enter method number:");
        if (methodIndex < 1 || methodIndex > methods.Count)
        {
            Console.WriteLine("Invalid method number.");
            return;
        }

        var methodInfo = methods[methodIndex - 1];
        var parameters = methodInfo.GetParameters();
        var arguments = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            string parameterName = parameters[i].Name;
            Type parameterType = parameters[i].ParameterType;
            string prompt = $"Enter {parameterName} ({parameterType.Name}):";
            string input = ReadString(prompt);
            try
            {
                arguments[i] = Convert.ChangeType(input, parameterType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to convert {input} to type {parameterType.Name}: {ex.Message}");
                return;
            }
        }

        methodInfo.Invoke(instance, arguments);
    }

    static void RunCommandMode(string[] args)
    {
        string className = args[0];
        string methodName = args[1];

        Type type = Type.GetType(className, false, true);
        if (type == null)
        {
            type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => t.IsPublic && t.Name.StartsWith(className))
                .FirstOrDefault();

            if (type == null)
            {
                Console.WriteLine($"Class {className} not found.");
                return;
            }
        }

        object instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            Console.WriteLine($"Failed to create instance of {className}.");
            return;
        }

        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        MethodInfo method = methods.FirstOrDefault(m => m.Name == methodName);
        if (method == null)
        {
            Console.WriteLine($"Method {methodName} not found in class {className}.");
            return;
        }

        var parameters = method.GetParameters();
        var arguments = new object[parameters.Length];

        if (args.Length != parameters.Length + 2)
        {
            Console.WriteLine($"Wrong number of arguments. Method {methodName} in class {className} expects {parameters.Length} argument(s).");
            return;
        }

        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            string input = args[i + 2];
            try
            {
                arguments[i] = Convert.ChangeType(input, parameterType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to convert {input} to type {parameterType.Name}: {ex.Message}");
                return;
            }
        }

        method.Invoke(instance, arguments);
    }

    static List<Type> GetPublicClasses()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsPublic && t.IsClass)
            .ToList();
    }

    static List<MethodInfo> GetPublicInstanceMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
    }

    static int ReadInt(string prompt)
    {
        int result;
        while (true)
        {
            Console.Write(prompt + " ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out result))
            {
                return result;
            }
            else
            {
                Console.WriteLine($"Invalid number: {input}");
            }
        }
    }

    static string ReadString(string prompt)
    {
        Console.Write(prompt + " ");
        return Console.ReadLine();
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: dotnet run <class_name> <method_name> [<arg1> <arg2> ...]");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run MyClass MyMethod");
        Console.WriteLine("  dotnet run MyNamespace.MyClass MyMethod 42 \"hello world\"");
        Console.WriteLine();
        Console.WriteLine("Interactive mode:");
        Console.WriteLine("  dotnet run interactive");
    }
}