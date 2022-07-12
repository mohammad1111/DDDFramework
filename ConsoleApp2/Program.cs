namespace ConsoleApp2;

internal class Program
{
    private static readonly string[] My = { ".net" };

    private static void Main(string[] args)
    {
        var s = My.First(x => x == ".net");
        Console.WriteLine($"Welcome to {s} 6");
        Console.ReadLine();
    }
}