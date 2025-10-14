namespace App;

//JournalService

//This class is responsible for managing patient journal entires.
//It simulates a database by storing each patient's journal
//as a separate .txt file inside the "Data/" directory.

//Each journal entry is stored as plain test, one line per entry.
//Example file path: Data/Journal_3.txt (for patient with ID3)

public class JournalService
{
  //Base directory for all journal files.
  //If the directory does not exist, it will be created automatically.
  private readonly string _baseDir = "Data";

  //Loads all journal entries for a specific user.

  //Parameyers:
  //userId - The ID of the user whose journal should be loaded.

  //returns:
  //A list of journal entries (strings), or an empty list if no file exists.

  public List<string> LoadJournal(int userId)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.txt");

    //Ensure that the Data directory exists.
    if (!Directory.Exists(_baseDir))
      Directory.CreateDirectory(_baseDir);

    //If no journal file exists yet, return an empty list.
    if (!File.Exists(filePath))
      return new List<string>();

    //Read all lines and return them as a list
    return File.ReadAllLines(filePath).ToList();
  }

  // Saves a new entry to the user's journal file.
  // Parameters:
  //   userId - The ID of the user whose journal to update.
  //   entry  - The journal text to append (one entry = one line).
  //
  // Each entry is timestamped for traceability.
  // Example:
  //   [2025-10-14 13:45] Blood pressure check-up completed.
  public void SaveJournalEntry(int userId, string entry)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.txt");

    // Ensure directory exists
    if (!Directory.Exists(_baseDir))
      Directory.CreateDirectory(_baseDir);

    // Add timestamp to each entry
    string timestampedEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm}] {entry}";

    // Append the entry to the journal file
    File.AppendAllText(filePath, timestampedEntry + Environment.NewLine);
  }

  // ==========================================
  // Deletes all journal entries for a given user.
  //
  // Used only in cases where an account or data reset is required.
  // ==========================================
  public void ClearJournal(int userId)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.txt");

    if (File.Exists(filePath))
      File.Delete(filePath);
  }
}