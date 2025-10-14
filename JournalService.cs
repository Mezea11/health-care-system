using System.Text.Json;

namespace App;

//JournalService 

//Handles reading and writing of patient journals
//to JSON files instead of plain text files.
//Each user's journal is stored as an array of entires:
//"Date": "2025-10-14T12:34:56", Text: "Blood pressure check-up."

public class JournalService
{
  private readonly string _baseDir = "Data";

  //Simple reacord to represent each jounral entry
  public class JournalEntry
  {
    public DateTime Date { get; set; }
    public string Text { get; set; }
  }

  //Loads all journal entries for a specific user.
  //Returns an empty list if file does not exist.

  public List<JournalEntry> LoadJournal(int userId)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.json");

    if (!Directory.Exists(_baseDir))
      Directory.CreateDirectory(_baseDir);

    if (!File.Exists(filePath))
      return new List<JournalEntry>();

    string json = File.ReadAllText(filePath);

    return JsonSerializer.Deserialize<List<JournalEntry>>(json)
    ?? new List<JournalEntry>();
  }

  //Saves a new journal entry (adds to existing list)
  //and writes ti back to the user's JSON file.

  public void SaveJournalEntry(int userId, string text)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.json");

    var entires = LoadJournal(userId);
    entires.Add(new JournalEntry
    {
      Date = DateTime.Now,
      Text = text
    });
    string json = JsonSerializer.Serialize(entires, new JsonSerializerOptions
    {
      WriteIndented = true
    });
    File.WriteAllText(filePath, json);
  }

  //Clears all journal entries for a given user.

  public void ClearJournal(int userId)
  {
    string filePath = Path.Combine(_baseDir, $"journal_{userId}.json");

    if (File.Exists(filePath))
      File.Delete(filePath);
  }
}