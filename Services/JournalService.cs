namespace App
{
 
  // Represents a single journal entry written by a staff member
 
  public class JournalEntry
  {
    public DateTime Timestamp { get; set; }
    public string Author { get; set; }
    public string Text { get; set; }

    public JournalEntry() { }

    public JournalEntry(string author, string text)
    {
      Timestamp = DateTime.Now;
      Author = author;
      Text = text;
    }

    // Returns a readable string for console display
    public string Format()
    {
      return $"{Timestamp:yyyy-MM-dd HH:mm} - {Author}: {Text}";
    }

    // Converts an entry to a single text line for file saving
    public string ToFileString(int patientId)
    {
      return $"{patientId};{Timestamp:yyyy-MM-dd HH:mm};{Author};{Text}";
    }

    // Creates a JournalEntry from a text line
    public static JournalEntry? FromFileString(string line)
    {
      if (string.IsNullOrWhiteSpace(line))
        return null;

      string[] parts = line.Split(';', 4);
      if (parts.Length < 4)
        return null;

      return new JournalEntry
      {
        Timestamp = DateTime.Parse(parts[1]),
        Author = parts[2],
        Text = parts[3]
      };
    }
  }

  
  // Represents all journal entries belonging to one patient
  
  public class PatientJournal
  {
    public int PatientId { get; set; }
    public List<JournalEntry> Entries { get; set; }

    public PatientJournal()
    {
      Entries = new List<JournalEntry>();
    }

    public PatientJournal(int patientId)
    {
      PatientId = patientId;
      Entries = new List<JournalEntry>();
    }
  }

 
  // JournalService
  // Handles reading and writing of journals to a text file
  
  public class JournalService
  {
    private readonly string _filePath = "Data/journals.txt";
    private List<PatientJournal> _journals = new List<PatientJournal>();

    public JournalService()
    {
      LoadJournals();
    }

 
    // Load all journals from the text file
    
    private void LoadJournals()
    {
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      if (!File.Exists(_filePath))
      {
        _journals = new List<PatientJournal>();
        return;
      }

      var lines = File.ReadAllLines(_filePath);
      var groupedByPatient = lines
        .Select(line => line.Split(';', 4))
        .Where(parts => parts.Length == 4)
        .GroupBy(parts => int.Parse(parts[0]));

      _journals = new List<PatientJournal>();

      foreach (var group in groupedByPatient)
      {
        int patientId = group.Key;
        var journal = new PatientJournal(patientId);

        foreach (var parts in group)
        {
          var entry = new JournalEntry
          {
            Timestamp = DateTime.Parse(parts[1]),
            Author = parts[2],
            Text = parts[3]
          };
          journal.Entries.Add(entry);
        }

        _journals.Add(journal);
      }
    }

   
    // Save all journals to the text file
    
    private void SaveJournals()
    {
      var lines = new List<string>();

      foreach (var journal in _journals)
      {
        foreach (var entry in journal.Entries)
        {
          lines.Add(entry.ToFileString(journal.PatientId));
        }
      }

      File.WriteAllLines(_filePath, lines);
    }

    
    // Get all journal entries for a specific patient
   
    public List<JournalEntry> GetJournalEntries(int patientId)
    {
      var journal = _journals.FirstOrDefault(j => j.PatientId == patientId);
      return journal?.Entries ?? new List<JournalEntry>();
    }


    // Add a new entry to a patient's journal
 
    public void AddEntry(int patientId, string author, string text)
    {
      var existingJournal = _journals.FirstOrDefault(j => j.PatientId == patientId);

      if (existingJournal == null)
      {
        existingJournal = new PatientJournal(patientId);
        _journals.Add(existingJournal);
      }

      var newEntry = new JournalEntry(author, text);
      existingJournal.Entries.Add(newEntry);
      SaveJournals();
    }

 
    // Notification system for patient updates

    public class NotificationService
    {
      public void NotifyPatient(int patientId, string message)
      {
        Console.WriteLine($"\n[NOTIFICATION] Patient #{patientId}: {message}\n");
      }
    }
  }
}
