namespace App
{
  public class Appointment
  {
    public int UserId { get; set; } // The ID of the user (e.g., patient) who owns this appointment.
    public DateTime Date { get; set; } // The date and time when the appointment takes place.
    public string Doctor { get; set; } // The name of the doctor responsible for the appointment.
    public string Department { get; set; } // The department or medical unit where the appointment occurs.
    public string Type { get; set; } // The type of appointment (e.g., "Checkup", "Follow-up", "Consultation").

    // ✨ Nya fält:
    public string Status { get; set; } = "Pending"; // Default state
    public int? PersonnelId { get; set; } // Optional link to staff who created/owns the appointment

    // Constructor for creating a new Appointment object with all necessary details.
    public Appointment(int userId, DateTime date, string doctor, string department, string type)
    {
      UserId = userId;
      Date = date;
      Doctor = doctor;
      Department = department;
      Type = type;
    }

    // Returns a clean, formatted string representation of the appointment.
    public string Format()
    {
      return $"Date: {Date:yyyy-MM-dd HH:mm} | Doctor: {Doctor,-15} | Location: {Department,-15} | For What: {Type} | Status: {Status}";
    }

    // Converts the appointment into a line of text (if still used for file saving)
    public string ToFileString()
    {
      return $"{UserId};{Date:yyyy-MM-dd HH:mm};{Doctor};{Department};{Type};{Status}";
    }

    // Creates an Appointment object from a line of text (for backward compatibility)
    public static Appointment FromFileString(string line)
    {
      string[] parts = line.Split(';');
      if (parts.Length < 5) return null;

      int userId = int.Parse(parts[0]);
      DateTime date = DateTime.Parse(parts[1]);
      string doctor = parts[2];
      string department = parts[3];
      string type = parts[4];

      Appointment app = new Appointment(userId, date, doctor, department, type);
      if (parts.Length >= 6) app.Status = parts[5];

      return app;
    }
  }
}
