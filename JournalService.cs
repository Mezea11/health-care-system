using System.Text.Json;

namespace App;

//JournalService 

//Service: Handles saving/loading of patient journals.
//Each journal is tied to a specific patient (userID).
//Personnel can only access journals for patients assiigned to them.

//Represents one single journal entry.

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
  public string Format()
  {
    return $"{Date:yyyy-MM-dd HH:mm} - {Author}: {Text}";
  }
}

//Represents a patient's journal containing muliple entires.

public class PatientJournal
{
  public int PatientId { get; set; }
  public List<JournalEntry> Entries { get; set; } = new();
  public PatientJournal() { }
  public PatientJournal(int patientId)
  {
    PatientId = patientId;
  }
}

//JournalService - manages saving/loading journals

public class JournalService
{
  private string _filePath = "Date/journals.json";
  private List<PatientJournal> _journals = new List<PatientJournal>();

  public JournalService()
  {
    LoadJournals();
  }
  //Load all journals from JSON
  private void LoadJournals()
  {
    if (!File.Exists(_filePath))
    {
      _journals = new List<PatientJournal>();
      return;
    }
    string json = File.ReadAllText(_filePath);
    _journals = JsonSerializer.Deserialize<List<PatientJournal>>(json) ?? new List<PatientJournal>();
  }
  //Save all journals to JSON
  private void SaveJournals()
  {
    string json = JsonSerializer.Serialize(_journals, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(_filePath, json);
  }
  //Get all entries for a specific patient
  public List<JournalEntry> GetJournalEntries(int patientId)
  {
    var journal = _journals.FirstOrDefault(j => j.PatientId == patientId);
    return journal?.Entries ?? new List<JournalEntry>();
  }
  //Add a new entry to a patient's journal
  public void AddEntry(int patientId, string author, string text)
  {
    var journal = _journals.FirstOrDefault(j => j.PatientId == patientId);

    if (journal == null)
    {
      journal = new PatientJournal(patientId);
      _journals.Add(journal);
    }

    journal.Entries.Add(new JournalEntry(author, text));
    SaveJournals();
  }
}
