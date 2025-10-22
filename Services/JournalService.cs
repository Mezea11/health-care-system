namespace App
{
  // Handles patient journal entries: loading, adding, and formatting
  public class JournalService
  {
    private readonly string _journalFile = "Data/journals.txt";

    public JournalService()
    {
      EnsureDataDirectoryExists();
    }

    // --- Get all journal entries for a patient ---
    public List<JournalEntry> GetJournalEntries(int patientId)
    {
      var entries = new List<JournalEntry>();

      if (!File.Exists(_journalFile)) return entries;

      var lines = File.ReadAllLines(_journalFile);
      foreach (var line in lines)
      {
        var entry = JournalEntry.FromFileString(line);
        if (entry != null && entry.PatientId == patientId)
        {
          entries.Add(entry);
        }
      }

      return entries;
    }

    // --- Add a new journal entry for a patient ---
    // --- Add a new journal entry for a patient ---
    public void AddEntry(int patientId, string authorName, string entryText)
    {
      var newEntry = new JournalEntry
      {
        PatientId = patientId,       // Patient ID
        AuthorName = authorName,     // Use correct property name
        EntryText = entryText,       // Use correct property name
        CreatedAt = DateTime.Now     // Use correct property name
      };

      // Append the entry to the journal file
      File.AppendAllLines(_journalFile, new[] { newEntry.ToFileString() });
    }


    // Ensure the data directory exists
    private void EnsureDataDirectoryExists()
    {
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");
    }
  }

  // Represents a single journal entry


  // Represents a single journal entry for a patient
  public class JournalEntry
  {
    public int PatientId { get; set; }           // ID of the patient
    public DateTime CreatedAt { get; set; }      // Timestamp when entry was created
    public string AuthorName { get; set; } = ""; // Who wrote the entry
    public string EntryText { get; set; } = "";  // Content of the journal entry

    // Format for console display
    public string Format()
    {
      return $"[{CreatedAt:yyyy-MM-dd HH:mm}] {AuthorName}: {EntryText}";
    }

    // Convert journal entry to string for saving to file
    public string ToFileString()
    {
      return $"{PatientId};{CreatedAt:yyyy-MM-dd HH:mm};{AuthorName};{EntryText}";
    }

    // Create a JournalEntry object from a line read from file
    public static JournalEntry? FromFileString(string line)
    {
      if (string.IsNullOrWhiteSpace(line)) return null;

      var parts = line.Split(';');
      if (parts.Length < 4) return null;

      if (!int.TryParse(parts[0], out int patientId)) return null;
      if (!DateTime.TryParse(parts[1], out DateTime createdAt)) return null;

      return new JournalEntry
      {
        PatientId = patientId,
        CreatedAt = createdAt,
        AuthorName = parts[2],
        EntryText = parts[3]
      };
    }
  }
}




