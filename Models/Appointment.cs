using System;

namespace App
{
  // Represents a single appointment in the healthcare system.
  public class Appointment
  {
    // --- Properties ---

    public int UserId { get; set; } // The ID of the patient or user who owns this appointment.
    public DateTime Date { get; set; } // The date and time of the appointment.
    public string Doctor { get; set; } // The doctor responsible for the appointment.
    public string Department { get; set; } // The department where the appointment occurs.
    public string Type { get; set; } // The type of appointment (e.g., "Checkup", "Follow-up").
    public string Status { get; set; } = "Pending"; // Default status.
    public int? PersonnelId { get; set; } // Optional link to staff.
    public bool IsApproved { get; set; } = false; // Approval flag.

    public Registration RegistrationPending { get; set; } = Registration.Pending;

    // --- Constructors ---

    public Appointment() { }

    public Appointment(int userId, DateTime date, string doctor, string department, string type)
    {
      UserId = userId;
      Date = date;
      Doctor = doctor;
      Department = department;
      Type = type;
      Status = "Pending";
      RegistrationPending = Registration.Pending;
      IsApproved = false;
    }

    // --- Methods ---

    // Returns a formatted string for console output.
    public string Format()
    {
      return $"Date: {Date:yyyy-MM-dd HH:mm} | Doctor: {Doctor,-15} | Dept: {Department,-15} | Type: {Type,-12} | Status: {Status}";
    }

    // Converts appointment to a single-line string for saving to a file.
    public string ToFileString()
    {
      return $"{UserId};{Date:yyyy-MM-dd HH:mm};{Doctor};{Department};{Type};{Status};{(PersonnelId.HasValue ? PersonnelId.Value.ToString() : "")}";
    }

    // Creates an Appointment object from a line of text.
    public static Appointment? FromFileString(string line)
    {
      if (string.IsNullOrWhiteSpace(line))
        return null;

      string[] parts = line.Split(';');
      if (parts.Length < 5)
        return null;

      int userId = int.Parse(parts[0]);
      DateTime date = DateTime.Parse(parts[1]);
      string doctor = parts[2];
      string department = parts[3];
      string type = parts[4];

      var appointment = new Appointment(userId, date, doctor, department, type);

      if (parts.Length > 5)
        appointment.Status = parts[5];
      if (parts.Length > 6 && int.TryParse(parts[6], out int personnelId))
        appointment.PersonnelId = personnelId;

      return appointment;
    }
  }
}
