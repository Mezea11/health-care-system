namespace App;

/// <summary>
/// Service class responsible for loading and saving schedules from a text file.
/// This simulates a database by storing appointments in a plain text format.
/// </summary>
public class ScheduleService
{
  // Path to the text file that acts as the mock database.
  private readonly string _filePath = "Data/schedule.txt";

  /// <summary>
  /// Loads a user's schedule by reading all appointments that belong to a given user ID.
  /// Only appointments that match the provided userId will be included.
  /// </summary>
  /// <param name="userId">The ID of the user whose schedule should be loaded.</param>
  /// <returns>A Schedule object containing all appointments for that user.</returns>
  public Schedule LoadSchedule(int userId)
  {
    // Create a new empty schedule for the user.
    Schedule schedule = new Schedule(userId);

    // If no schedule file exists yet, simply return the empty schedule.
    if (!File.Exists(_filePath))
      return schedule;

    // Read all lines (appointments) from the file.
    string[] lines = File.ReadAllLines(_filePath);

    // Loop through each line and attempt to parse it into an Appointment object.
    foreach (string line in lines)
    {
      // Each line should be in the format:
      // userId;date;doctor;department;type
      string[] parts = line.Split(';');

      // Skip malformed lines that don't have enough data.
      if (parts.Length < 5)
        continue;

      // Try to parse the userId from the file line.
      if (!int.TryParse(parts[0], out int fileUserId))
        continue;

      // Only include appointments that belong to the requested user.
      if (fileUserId != userId)
        continue;

      // Parse the rest of the data fields.
      DateTime date = DateTime.Parse(parts[1]);
      string doctor = parts[2];
      string department = parts[3];
      string type = parts[4];

      // Create a new Appointment instance and add it to the user's schedule.
      Appointment appointment = new Appointment(fileUserId, date, doctor, department, type);
      schedule.AddAppointment(appointment);
    }

    // Return the completed schedule, now containing all relevant appointments.
    return schedule;
  }

  /// <summary>
  /// Saves a new appointment by appending it to the schedule text file.
  /// This acts as inserting a new record into the mock database.
  /// </summary>
  /// <param name="appointment">The appointment to save.</param>
  public void SaveAppointment(Appointment appointment)
  {
    // Ensure that the directory exists to prevent file errors.
    string? directory = Path.GetDirectoryName(_filePath);
    if (!string.IsNullOrEmpty(directory))
      Directory.CreateDirectory(directory);

    // Convert the appointment to a text line in this order:
    // userId;date;doctor;department;type
    string line = $"{appointment.UserId};{appointment.Date:yyyy-MM-dd HH:mm};{appointment.Doctor};{appointment.Department};{appointment.Type}";

    // Append the new appointment to the file.
    File.AppendAllText(_filePath, line + Environment.NewLine);
  }
}
