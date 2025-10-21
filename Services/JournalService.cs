namespace App
{
 
  // Represents a single journal entry written by a staff member
 
  public class JournalEntry
  {
    public DateTime Timestamp { get; set; } // Timestamp when the entry was created 
    public string Author { get; set; } // Name of the author (staff or personnel) who created the entry
    public string Text { get; set; } // Content of the journal entry

    public JournalEntry() { } // Default constructor (required for desarialization)

    public JournalEntry(string author, string text) // Contstructor to create a new journal entry with author and text
    {
      Timestamp = DateTime.Now; // Automatically set creation time to now
      Author = author; // Set author name
      Text = text; // Set journal text
    }

    // Returns a formatted string for console display or quick reading
    public string Format()
    {
      return $"{Timestamp:yyyy-MM-dd HH:mm} - {Author}: {Text}";
    }

    // Converts an entry to a single text line for file saving
    public string ToFileString(int patientId)
    {
      return $"{patientId};{Timestamp:yyyy-MM-dd HH:mm};{Author};{Text}";
    }

    
    public static JournalEntry? FromFileString(string line) // Creates a JournalEntry object from a single line of text in the file 
    {
      if (string.IsNullOrWhiteSpace(line)) // If the line is empty or contains only whitespace, return null (inavlid data)
        return null;

      string[] parts = line.Split(';', 4); // Split the line into parts separated by semicolons, The 4 ensures the text part (which may contain samicolons) stays intact as one piece
      if (parts.Length < 4) // Check if the line contains all required parts (patientID, timestamp, authorm text)
        return null;

      return new JournalEntry // Create and return a new JournalEntry object using the parsed data
      {
        Timestamp = DateTime.Parse(parts[1]), // Convert the timestamp string to a DateTime object
        Author = parts[2], // Set the author name
        Text = parts[3] // Set the txt content if the entry
      };
    }
  }

  
  public class PatientJournal // Represents all journal entries that belong to a single patient
  {
    public int PatientId { get; set; } // The unique ID of the patient this journal belongs to 
    public List<JournalEntry> Entries { get; set; } // A list that conatins all journal entries written for this patient

    public PatientJournal() // Default constructor - creates an empty journal with no patient assigned yet
    {
      Entries = new List<JournalEntry>();
    }

    public PatientJournal(int patientId) // Constructor that initializes a new journal for a specific patient
    {
      PatientId = patientId;
      Entries = new List<JournalEntry>();
    }
  }

 
  // The JournalService class manages saving and loading of patient journals
  // to and from text file. It ensures that all journal data is stored persistently and reloaded when needed
  public class JournalService
  {
    private readonly string _filePath = "Data/journals.txt"; // File path where all journals are saved
    private List<PatientJournal> _journals = new List<PatientJournal>(); // A list holding all loaded patient journals in memory

    public JournalService() // Constructor - automatically loads all journals from the file, when a new JournalService instance is created
    {
      LoadJournals();
    }

 
    // Load all journal data from the text file into memory, each line in the file represents one journal entry
    
    private void LoadJournals()
    {
      if (!Directory.Exists("Data")) // Ensure the Data directory exists; create it if missing
        Directory.CreateDirectory("Data");

      if (!File.Exists(_filePath)) // If the journal file does not exist, create an empty list in memory
      {
        _journals = new List<PatientJournal>();
        return;
      }

      var lines = File.ReadAllLines(_filePath); // Read all lines from the text file
      var groupedByPatient = lines // Split each line into parts and group entries by PatientID, Each group represents all journal entries belonging to one patient
        .Select(line => line.Split(';', 4)) // Split into 4 sections
        .Where(parts => parts.Length == 4) // Ensure each line has valid data
        .GroupBy(parts => int.Parse(parts[0])); // Group by Patient Id 

      _journals = new List<PatientJournal>(); // Prepare a new list to hold all reconstructed PatientJournal object

      foreach (var group in groupedByPatient) // Process each patient group and rebuild their journal entries
      {
        int patientId = group.Key; // The patient this group belongs to
        var journal = new PatientJournal(patientId);

        foreach (var parts in group) // Recreate each journal entry and add it to the patient's journal
        {
          var entry = new JournalEntry
          {
            Timestamp = DateTime.Parse(parts[1]), // When the entry was written
            Author = parts[2], // Who wrote it
            Text = parts[3] // The content of the entry
          };
          journal.Entries.Add(entry);
        }

        _journals.Add(journal); // Add the completed journal to the in-memory list of all journals
      }
    }

    
    private void SaveJournals() // Saves all journals currently in memory (_journals list) to the text file, Each journal entry is written as a single line in the format
    {
      var lines = new List<string>(); // Create a temporary list to hold all lines that will be saved

      foreach (var journal in _journals) // Loop through every patient's journal
      {
        foreach (var entry in journal.Entries) // loop through each entry beloning to this patient 
        {
          lines.Add(entry.ToFileString(journal.PatientId)); // Convert the entry to a formatted text line for file storage
        }
      }

      File.WriteAllLines(_filePath, lines); // Overwrite the journals file with the updated list of entries
    }

    
    // Retrieves all journal entries for a speific patient, returns an empty list if the patient does not have a journal yet
    public List<JournalEntry> GetJournalEntries(int patientId)
    {
      var journal = _journals.FirstOrDefault(j => j.PatientId == patientId); // Search for a journal beloning to the given patient ID
      return journal?.Entries ?? new List<JournalEntry>(); // Return the found journal's entrires or an empty list if none exist
    }


    // Adds a new journal entry for a given patient 
    public void AddEntry(int patientId, string author, string text)
    {
      var existingJournal = _journals.FirstOrDefault(j => j.PatientId == patientId); // Look for an existing journal for this patient

      if (existingJournal == null) // If no journal exists, create a new one and add it to the list
      {
        existingJournal = new PatientJournal(patientId);
        _journals.Add(existingJournal);
      }

      var newEntry = new JournalEntry(author, text); // Create a new entry with the provided author and text
      existingJournal.Entries.Add(newEntry); // Add the entry to the patient's journal in memory
      SaveJournals(); //Save all journals back to the text file to perist the new entry
    }

    // NotificationService, Handles sending simple console notifications to patiens
    public class NotificationService
    {
      public void NotifyPatient(int patientId, string message) // Displays a simulated notification message to specific patient
      {
        Console.WriteLine($"\n[NOTIFICATION] Patient #{patientId}: {message}\n");
      }
    }
  }
}
