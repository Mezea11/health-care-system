namespace App;

// Represents a user's complete schedule.
// A schedule contains all appointments that belong to a specific user(patient or staff)
public class Schedule
{
  public int UserId; // The ID of the user who owns this schedule.
  public List<Appointment> Appointments; // A list that stores all appointments for this user.


  // Constructor for creating a new empty schedule for a user.

  // "userId" = The ID of the logged-in user.
  public Schedule(int userId)
  {
    UserId = userId;
    Appointments = new List<Appointment>();
  }

  //Adds an appointment to the user's schedule.
  //Only adds the appointment if it belongs to the correct user
  public void AddAppointment(Appointment appointment)
  {
    if (appointment.UserId == UserId)
    {
      Appointments.Add(appointment);
    }
  }
  public List<Appointment> GetAppointmentsForWeek(DateTime weekStart)
  {
    DateTime weekEnd = weekStart.AddDays(7);
    return Appointments
    .Where(a => a.Date >= weekStart && a.Date < weekEnd)
    .OrderBy(a => a.Date)
    .ToList();
  }

  public void PrintSchedule()
  {
    Console.WriteLine($"Schedule for user #{UserId}:");
    foreach (var a in Appointments.OrderBy(a => a.Date))
    {
      Console.WriteLine(a.Format());
    }
  }
}