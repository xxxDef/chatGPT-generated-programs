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

    private static void RunCommandMode(string[] args)
{
    if (args.Length == 0 || args[0].ToLower() == "help")
    {
        PrintHelp();
        return;
    }

    if (args[0].ToLower() == "interactive")
    {
        RunInteractiveMode();
        return;
    }

    Type type = GetPublicClasses().FirstOrDefault(t => t.Name.ToLower() == args[0].ToLower() || t.Name.ToLower() == args[0].ToLower() + "class");

    if (type == null)
    {
        Console.WriteLine($"Error: class \"{args[0]}\" not found.");
        Console.WriteLine();
        PrintHelp();
        return;
    }

    MethodInfo method = GetPublicInstanceMethods(type).FirstOrDefault(m => m.Name.ToLower() == args[1].ToLower());

    if (method == null)
    {
        Console.WriteLine($"Error: method \"{args[1]}\" not found in class \"{type.Name}\".");
        Console.WriteLine();
        PrintMethods(type);
        return;
    }

    ParameterInfo[] parameters = method.GetParameters();
    object[] arguments = new object[parameters.Length];

    if (args.Length - 2 != parameters.Length)
    {
        Console.WriteLine($"Error: wrong number of arguments for method \"{method.Name}\".");
        Console.WriteLine();
        PrintMethodUsage(method);
        return;
    }

    for (int i = 0; i < parameters.Length; i++)
    {
        try
        {
            arguments[i] = Convert.ChangeType(args[i + 2], parameters[i].ParameterType);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Error: invalid value for parameter \"{parameters[i].Name}\".");
            Console.WriteLine();
            PrintMethodUsage(method);
            return;
        }
    }

    object instance = Activator.CreateInstance(type);
    method.Invoke(instance, arguments);
}



    static List<Type> GetPublicClasses()
    {
        List<Type> classes = new List<Type>();
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass && type.IsPublic)
            {
                classes.Add(type);
            }
        }

        return classes;
    }

private static void PrintMethodUsage(MethodInfo method)
{
    Console.WriteLine($"Usage: {method.DeclaringType.Name}.{method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");
}
    static List<MethodInfo> GetPublicInstanceMethods(Type type)
    {
        List<MethodInfo> methods = new List<MethodInfo>();

        foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (method.DeclaringType == type)
            {
                methods.Add(method);
            }
        }

        return methods;
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
        Console.WriteLine("Available classes:");

        List<Type> classes = GetPublicClasses();
        for (int i = 0; i < classes.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {classes[i].FullName}");
            PrintMethods(classes[i]);
        }

        Console.WriteLine();
        Console.WriteLine("Interactive mode:");
        Console.WriteLine("  dotnet run interactive");
    }


    static void PrintMethods(Type type)
    {
        Console.WriteLine($"Methods of class {type.FullName}:");
        List<MethodInfo> methods = GetPublicInstanceMethods(type);
        for (int i = 0; i < methods.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {methods[i].Name}({string.Join(", ", methods[i].GetParameters().Select(p => p.ParameterType.Name))})");
        }
    }

}