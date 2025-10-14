
using System.Text.Json;


namespace App
{
  // ==========================================
  // ScheduleService (JSON version)
  // ------------------------------------------
  // Responsible for loading and saving user schedules
  // to and from a JSON file.
  //
  // File format: Data/schedules.json
  // [
  //   { "UserId": 1, "Date": "2025-10-14T08:00:00", "Doctor": "Dr. Smith", "Department": "Cardiology", "Type": "Checkup" },
  //   { "UserId": 2, "Date": "2025-10-15T10:00:00", "Doctor": "Dr. Brown", "Department": "Neurology", "Type": "Consultation" }
  // ]
  // ==========================================
  public class ScheduleService
  {
    private readonly string _filePath = "Data/schedules.json";

    // ==========================================
    // Loads a full schedule for a specific user.
    //
    // Parameters:
    //   userId - The ID of the user whose schedule should be loaded.
    //
    // Returns:
    //   A Schedule object containing all appointments for the user.
    // ==========================================
    public Schedule LoadSchedule(int userId)
    {
      // Ensure directory exists
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      // Create an empty schedule by default
      Schedule schedule = new Schedule(userId);

      // If no JSON file exists, return an empty schedule
      if (!File.Exists(_filePath))
        return schedule;

      // Read file contents
      string json = File.ReadAllText(_filePath);

      // Deserialize JSON into a list of appointments
      List<Appointment>? allAppointments =
          JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();

      // Filter appointments that belong to this user
      var userAppointments = allAppointments
          .Where(a => a.UserId == userId)
          .OrderBy(a => a.Date)
          .ToList();

      // Add appointments to the user's schedule
      foreach (var app in userAppointments)
      {
        schedule.AddAppointment(app);
      }

      return schedule;
    }

    // ==========================================
    // Saves (or appends) a new appointment to the JSON file.
    //
    // If the file does not exist, it creates it.
    // ==========================================
    public void SaveAppointment(Appointment appointment)
    {
      // Ensure directory exists
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      List<Appointment> allAppointments = new();

      // Load existing data if available
      if (File.Exists(_filePath))
      {
        string existingJson = File.ReadAllText(_filePath);
        allAppointments = JsonSerializer.Deserialize<List<Appointment>>(existingJson)
                          ?? new List<Appointment>();
      }

      // Add the new appointment
      allAppointments.Add(appointment);

      // Serialize and overwrite the file
      string newJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      File.WriteAllText(_filePath, newJson);
    }

    // ==========================================
    // Removes an appointment by userId and date.
    //
    // Used for cancellation functionality.
    // ==========================================
    public void RemoveAppointment(int userId, DateTime date)
    {
      if (!File.Exists(_filePath))
        return;

      string json = File.ReadAllText(_filePath);

      List<Appointment>? allAppointments =
          JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();

      // Remove the specific appointment
      allAppointments.RemoveAll(a => a.UserId == userId && a.Date == date);

      // Save back to file
      string updatedJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      File.WriteAllText(_filePath, updatedJson);
    }
  }
}
