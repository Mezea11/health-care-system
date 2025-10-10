namespace App;

public class Schedule
{
  public int UserId;
  public List<Appointment> Appointments;

  public Schedule(int userId)
  {
    UserId = userId;
    Appointments = new List<Appointment>();
  }
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