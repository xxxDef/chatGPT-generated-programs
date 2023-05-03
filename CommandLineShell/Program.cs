using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintHelp();
            return;
        }

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

        var methodInfo = type.GetMethod(methodName);
        if (methodInfo == null)
        {
            Console.WriteLine($"Method {methodName} not found in class {className}.");
            return;
        }

        var parameters = methodInfo.GetParameters();
        if (args.Length - 2 != parameters.Length)
        {
            Console.WriteLine($"Method {methodName} requires {parameters.Length} arguments.");
            return;
        }

        var arguments = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            Type parameterType = parameters[i].ParameterType;
            string argumentString = args[i + 2];
            if (!TryConvertArgument(argumentString, parameterType, out object argumentValue))
            {
                Console.WriteLine($"Failed to convert argument {argumentString} to type {parameterType.Name}.");
                return;
            }
            arguments[i] = argumentValue;
        }

        methodInfo.Invoke(instance, arguments);
    }

    static void PrintHelp()
    {
        Console.WriteLine("Usage: Program <ClassName> <MethodName> [args...]");
        Console.WriteLine("Possible classes:");
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsPublic))
        {
            Console.WriteLine($"- {type.Name}");
            Console.WriteLine("  Possible methods:");
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                Console.Write($"  - {method.Name}(");
                Console.Write(string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}")));
                Console.WriteLine(")");
            }
        }
    }

    static bool TryConvertArgument(string argumentString, Type targetType, out object targetValue)
    {
        targetValue = null;

        if (targetType == typeof(string))
        {
            targetValue = argumentString;
            return true;
        }

        MethodInfo parseMethod = targetType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
        if (parseMethod != null && parseMethod.ReturnType == targetType)
        {
            try
            {
                targetValue = parseMethod.Invoke(null, new[] { argumentString });
                return true;
            }
            catch { }
        }

        return false;
    }
}
