namespace MyApp
{
    internal class Program
    {
        const int ExitCodeSuccess = 0;
        const int ExitCodeFailure = 1;

        internal static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Error: No user name provided.");
                return ExitCodeFailure;
            }
            else if (args.Length > 1)
                Console.Error.WriteLine("Warning: Ignoring excess arguments.");

            string userName = args[0];
            Console.WriteLine("Hello, {0}!", userName);

            return ExitCodeSuccess;
        }
    }
}