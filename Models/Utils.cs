namespace App;

class Utils
{
    // static int GetIndexAddOne(List<IUser> users) => users.Count + 1;
    public static int GetIndexAddOne(List<IUser> users)
    {
        return users.Count() + 1;
    }

    public static string GetRequiredInput(string promptMessage)
    {
        string? input = null;

        // Fortsätt fråga i en loop tills inmatningen är ett giltigt värde
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write(promptMessage);

            // Läs inmatning. Använd ?? "" för att garantera att 'input' aldrig är null.
            input = Console.ReadLine() ?? "";

            // Om inmatningen fortfarande är tom, visa ett felmeddelande.
            if (string.IsNullOrWhiteSpace(input))
            {
                // Använder din befintliga felmeddelande-funktion
                DisplayAlertText("The input could not be empthy. Try again");
                // Thread.Sleep(1000); // Kort paus innan frågan ställs igen
            }
        }

        // Returnera det validerade (icke-tomma) värdet
        return input;
    }

    public static void DisplaySuccesText(string text, int delay = 50)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{text}");
        Console.ResetColor(); // Clear the color        
    }

    /// <summary>
    /// Displays an alert message in red text.
    /// </summary>
    /// <param name="text">The message to display.</param>
    /// <param name="delay">Optional delay parameter (currently unused).</param>
    public static void DisplayAlertText(string text, int delay = 50)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{text}");
        Console.ResetColor(); // Clear the color        
    }

    public static int GetIntegerInput(string promptMessage)
    {
        while (true) // The loop is looping infinitly until the number is valid
        {
            Console.Write(promptMessage);
            string? input = Console.ReadLine();

            // Använder int.TryParse för att försöka konvertera till heltal
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            else
            {
                DisplayAlertText("Not accepteble input. User only numbers.Try again: ");
                // WaitForInput();
            }
        }
    }
}

