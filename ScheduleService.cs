using System.Text.Json;

namespace App
{
  // ==========================================
  // ScheduleService (JSON version)
  // ------------------------------------------
  // Responsible for loading and saving user schedules
  // to and from a JSON file.
  // ==========================================
  public class ScheduleService
  {
    private readonly string _filePath = "Data/schedules.json";

    // ==========================================
    // Loads a full schedule for a specific user.
    // ==========================================
    public Schedule LoadSchedule(int userId)
    {
      // Ensure directory exists
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      Schedule schedule = new Schedule(userId);

      if (!File.Exists(_filePath))
        return schedule;

      string json = File.ReadAllText(_filePath);

      // Only deserialize if file has content
      List<Appointment>? allAppointments = new();
      if (!string.IsNullOrWhiteSpace(json))
      {
        allAppointments = JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();
      }

      var userAppointments = allAppointments
          .Where(a => a.UserId == userId)
          .OrderBy(a => a.Date)
          .ToList();

      foreach (var app in userAppointments)
      {
        schedule.AddAppointment(app);
      }

      return schedule;
    }

    // ==========================================
    // Saves (or appends) a new appointment to the JSON file.
    // ==========================================

    //a public method that takes in one Appointment object
    // its job is to save this appointment to a file, so it's not lost when the app closes
    public void SaveAppointment(Appointment appointment)
    {

      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");

      List<Appointment> allAppointments = new();

      if (File.Exists(_filePath))
      {
        string existingJson = File.ReadAllText(_filePath);

        // Only deserialize if file is not empty
        if (!string.IsNullOrWhiteSpace(existingJson))
        {
          allAppointments = JsonSerializer.Deserialize<List<Appointment>>(existingJson)
                            ?? new List<Appointment>();
        }
      }

      allAppointments.Add(appointment);

      string newJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      File.WriteAllText(_filePath, newJson);
    }

    // ==========================================
    // Removes an appointment by userId and date.
    // ==========================================
    public void RemoveAppointment(int userId, DateTime date)
    {
      if (!File.Exists(_filePath))
        return;

      string json = File.ReadAllText(_filePath);

      List<Appointment>? allAppointments = new();
      if (!string.IsNullOrWhiteSpace(json))
      {
        allAppointments = JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();
      }

      allAppointments.RemoveAll(a => a.UserId == userId && a.Date == date);

      string updatedJson = JsonSerializer.Serialize(allAppointments, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      File.WriteAllText(_filePath, updatedJson);
    }
    public List<Appointment> LoadAllAppointments()
    {
      if (!File.Exists(_filePath))
        return new List<Appointment>();

      var lines = File.ReadAllLines(_filePath);
      var appointments = new List<Appointment>();

      foreach (var line in lines)
      {
        var appt = Appointment.FromFileString(line);
        if (appt != null)
          appointments.Add(appt);
      }
      return appointments;
    }
    //Removes a specific appointment from storage
    public void RemoveAppointment(Appointment apptToRemove)
    {
      var appointments = LoadAllAppointments();

      //Match by all key fields (unique combo)
      appointments.RemoveAll(a =>
      a.UserId == apptToRemove.UserId &&
      a.Date == apptToRemove.Date &&
      a.Doctor == apptToRemove.Doctor &&
      a.Department == apptToRemove.Department &&
      a.Type == apptToRemove.Type
      );

      //Re-save the updated list
      var lines = appointments.Select(a => a.ToFileString());
      File.WriteAllLines(_filePath, lines);
    }
  }
}
