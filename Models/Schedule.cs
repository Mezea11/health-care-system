namespace App
{
  
  // Represents a user's complete schedule.
  // A schedule contains all appointments that belong to one user
  // (either a patient or a staff member).
  
  public class Schedule
  {
    // --- Properties ---
    public int UserId { get; set; } // The ID of the user who owns this schedule.
    public List<Appointment> Appointments { get; set; } // All appointments for this user.

    // --- Constructor ---
    // Creates a new empty schedule for a specific user.
    public Schedule(int userId)
    {
      UserId = userId;
      Appointments = new List<Appointment>();
    }

    // Adds an appointment to the user's schedule.
    // Only adds it if the appointment belongs to the correct user.
    public void AddAppointment(Appointment appointment)
    {
      if (appointment == null)
      {
        Console.WriteLine("Tried to add a null appointment — ignored.");
        return;
      }

      if (appointment.UserId != UserId)
      {
        Console.WriteLine($"Appointment does not belong to user #{UserId} — ignored.");
        return;
      }

      Appointments.Add(appointment);
    }

    
    // Returns all appointments for a specific week.
    // The week starts at the given 'weekStart' date and includes 7 days.
    
    public List<Appointment> GetAppointmentsForWeek(DateTime weekStart)
    {
      DateTime weekEnd = weekStart.AddDays(7);

      var weeklyAppointments = Appointments
        .Where(appointment => appointment.Date >= weekStart && appointment.Date < weekEnd)
        .OrderBy(appointment => appointment.Date)
        .ToList();

      return weeklyAppointments;
    }

    
    // Prints the full schedule to the console in a readable format.
    // Mainly used for debugging, testing, or quick overviews.
    
    public void PrintSchedule()
    {
      Console.WriteLine($"\n--- Schedule for user #{UserId} ---");

      if (!Appointments.Any())
      {
        Console.WriteLine("No appointments found.");
        return;
      }

      foreach (var appointment in Appointments.OrderBy(appointment => appointment.Date))
      {
        Console.WriteLine(appointment.Format());
      }
    }
  }
}
