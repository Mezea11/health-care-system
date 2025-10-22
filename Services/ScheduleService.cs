

namespace App
{
  // Handles saving/loading appointments and personnel shifts
  public class ScheduleService
  {
    private readonly string _appointmentsFile = "Data/appointments.txt";
    private readonly string _shiftsFile = "Data/shifts.txt";

    // ----------------------
    // Appointments
    // ----------------------

    // Load schedule for a specific user (patient or personnel)
    public Schedule LoadSchedule(int userId)
    {
      EnsureDataDirectoryExists();
      var schedule = new Schedule(userId);

      if (!File.Exists(_appointmentsFile)) return schedule;

      var lines = File.ReadAllLines(_appointmentsFile);
      foreach (var line in lines)
      {
        var appointment = Appointment.FromFileString(line);
        if (appointment != null && appointment.UserId == userId)
          schedule.AddAppointment(appointment);
      }

      return schedule;
    }

    // Save or update an appointment
    public void SaveAppointment(Appointment appointment)
    {
      EnsureDataDirectoryExists();

      var appointmentLines = File.Exists(_appointmentsFile)
          ? File.ReadAllLines(_appointmentsFile).ToList()
          : new List<string>();

      // Remove existing appointment for same user/date
      appointmentLines.RemoveAll(existingLine =>
      {
        var existingAppointment = Appointment.FromFileString(existingLine);
        return existingAppointment != null &&
                     existingAppointment.UserId == appointment.UserId &&
                     existingAppointment.Date == appointment.Date;
      });

      appointmentLines.Add(appointment.ToFileString());
      File.WriteAllLines(_appointmentsFile, appointmentLines);
    }

    // Remove appointment
    public void RemoveAppointment(int userId, DateTime date)
    {
      if (!File.Exists(_appointmentsFile)) return;

      var appointmentLines = File.ReadAllLines(_appointmentsFile).ToList();
      appointmentLines.RemoveAll(line =>
      {
        var appointment = Appointment.FromFileString(line);
        return appointment != null && appointment.UserId == userId && appointment.Date == date;
      });

      File.WriteAllLines(_appointmentsFile, appointmentLines);
    }

    // Load all appointments
    public List<Appointment> LoadAllAppointments()
    {
      if (!File.Exists(_appointmentsFile)) return new List<Appointment>();
      return File.ReadAllLines(_appointmentsFile)
                 .Select(Appointment.FromFileString)
                 .Where(a => a != null)
                 .Cast<Appointment>()
                 .ToList();
    }

    // Load appointments assigned to a personnel
    public List<Appointment> LoadPersonnelSchedule(int personnelId)
    {
      return LoadAllAppointments()
          .Where(a => a.PersonnelId == personnelId)
          .OrderBy(a => a.Date)
          .ToList();
    }

    // ----------------------
    // Shifts
    // ----------------------

    public List<Shift> LoadAllShifts()
    {
      EnsureDataDirectoryExists();
      if (!File.Exists(_shiftsFile)) return new List<Shift>();

      return File.ReadAllLines(_shiftsFile)
                 .Select(Shift.FromFileString)
                 .Where(s => s != null)
                 .Cast<Shift>()
                 .ToList();
    }

    public List<Shift> LoadShiftsForPersonnel(int personnelId)
    {
      return LoadAllShifts()
          .Where(s => s.PersonnelId == personnelId)
          .OrderBy(s => s.Start)
          .ToList();
    }

    public void SaveShift(Shift shift)
    {
      EnsureDataDirectoryExists();
      var shiftLines = File.Exists(_shiftsFile) ? File.ReadAllLines(_shiftsFile).ToList() : new List<string>();
      shiftLines.Add(shift.ToFileString());
      File.WriteAllLines(_shiftsFile, shiftLines);
    }

    public List<Shift> GetColleaguesForShift(Shift currentShift)
    {
      return LoadAllShifts()
          .Where(otherShift =>
              otherShift.PersonnelId != currentShift.PersonnelId &&
              ((otherShift.Start >= currentShift.Start && otherShift.Start < currentShift.End) ||
               (otherShift.End > currentShift.Start && otherShift.End <= currentShift.End) ||
               (otherShift.Start <= currentShift.Start && otherShift.End >= currentShift.End)))
          .OrderBy(s => s.Start)
          .ToList();
    }

    private void EnsureDataDirectoryExists()
    {
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");
    }

    // ----------------------
    // Shift class
    // ----------------------
    public class Shift
    {
      public DateTime Start { get; set; }
      public DateTime End { get; set; }
      public int PersonnelId { get; set; }

      public Shift() { }

      public Shift(DateTime start, DateTime end, int personnelId)
      {
        Start = start;
        End = end;
        PersonnelId = personnelId;
      }

      public string ToFileString() => $"{Start:yyyy-MM-dd HH:mm};{End:yyyy-MM-dd HH:mm};{PersonnelId}";

      public static Shift? FromFileString(string line)
      {
        if (string.IsNullOrWhiteSpace(line)) return null;
        var parts = line.Split(';');
        if (parts.Length < 3) return null;
        if (!DateTime.TryParse(parts[0], out DateTime start)) return null;
        if (!DateTime.TryParse(parts[1], out DateTime end)) return null;
        if (!int.TryParse(parts[2], out int personnelId)) return null;
        return new Shift(start, end, personnelId);
      }
    }
  }
}
