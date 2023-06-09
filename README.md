Note: All code, text and other content in this readme were generated by ChatGPT, a large language model trained by OpenAI.

# C# Command Line Method Caller

This is a simple C# program that allows you to call public methods of any class defined in the program from the command line. You can provide the class name, method name, and any method arguments as command line arguments, and the program will call the specified method and print its return value to the console.

## Usage

To use the program, open a command prompt or terminal window and navigate to the directory where the executable file is located. Then, run the following command:

```
CommandLineMethodCaller.exe ClassName MethodName [Argument1] [Argument2] ...
```

Replace `ClassName` and `MethodName` with the name of the class and method you want to call, respectively. If the method takes any arguments, provide them after the method name, separated by spaces.

For example, if you have a class named `Calculator` with a method named `Add` that takes two `int` parameters, you can call it like this:

```
CommandLineMethodCaller.exe Calculator Add 2 3
```

The program will call the `Add` method of the `Calculator` class with the arguments `2` and `3`, and print the result (`5`) to the console.

### Interactive Mode

If you provide the command line argument `interactive`, the program will enter interactive mode, where you can select the class and method to call from a menu, and provide the method arguments interactively. To start interactive mode, run the following command:

```
CommandLineMethodCaller.exe interactive
```

### Help

If you run the program without any arguments, or with the `help` argument, the program will print a list of available classes and methods, along with their signatures, to the console. To print the help message, run the following command:

```
CommandLineMethodCaller.exe help
```

## Sample Classes and Methods

The program includes several sample classes and methods that you can use to test the program. Here's a brief overview of the available classes and methods:

### `Calculator`

- `Add(int a, int b)`: Adds two integers and returns the result.
- `Subtract(int a, int b)`: Subtracts the second integer from the first and returns the result.
- `Multiply(int a, int b)`: Multiplies two integers and returns the result.

### `StringUtils`

- `Reverse(string s)`: Reverses the characters in a string and returns the result.
- `ToUpper(string s)`: Converts the string to uppercase and returns the result.
- `ToLower(string s)`: Converts the string to lowercase and returns the result.

### `DateUtils`

- `GetWeekday(DateTime date)`: Returns the name of the weekday for the given date.
- `IsLeapYear(int year)`: Returns `true` if the given year is a leap year, `false` otherwise.

## License

This program is licensed under the MIT License. See the `LICENSE` file for details.