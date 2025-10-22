namespace App
{
  // Handles reading and writing of appointments and personnel shifts
  public class ScheduleService
  {
    // File paths for saved data
    private readonly string _appointmentsFile = "Data/appointments.txt";
    private readonly string _shiftsFile = "Data/shifts.txt";

    // 
    //                          APPOINTMENTS
    // 

    // Load all appointments for a specific user (patient or personnel)
    public Schedule LoadSchedule(int userId)
    {
      EnsureDataDirectoryExists();
      var schedule = new Schedule(userId);

      if (!File.Exists(_appointmentsFile))
        return schedule;

      var lines = File.ReadAllLines(_appointmentsFile);

      // Parse each line into an Appointment object
      foreach (var line in lines)
      {
        var appointment = Appointment.FromFileString(line);
        if (appointment != null && appointment.UserId == userId)
          schedule.AddAppointment(appointment);
      }

      return schedule;
    }

    // Save or update a single appointment
    public void SaveAppointment(Appointment appointment)
    {
      EnsureDataDirectoryExists();

      var appointmentLines = File.Exists(_appointmentsFile)
          ? File.ReadAllLines(_appointmentsFile).ToList()
          : new List<string>();

      // Remove any old record with same user and date
      appointmentLines.RemoveAll(existingLine =>
      {
        var existingAppointment = Appointment.FromFileString(existingLine);
        return existingAppointment != null &&
                     existingAppointment.UserId == appointment.UserId &&
                     existingAppointment.Date == appointment.Date;
      });

      // Add the updated appointment
      appointmentLines.Add(appointment.ToFileString());
      File.WriteAllLines(_appointmentsFile, appointmentLines);
    }

    // Remove an appointment by user and date
    public void RemoveAppointment(int userId, DateTime date)
    {
      if (!File.Exists(_appointmentsFile))
        return;

      var appointmentLines = File.ReadAllLines(_appointmentsFile).ToList();

      // Delete any matching appointment line
      appointmentLines.RemoveAll(line =>
      {
        var appointment = Appointment.FromFileString(line);
        return appointment != null &&
                     appointment.UserId == userId &&
                     appointment.Date == date;
      });

      File.WriteAllLines(_appointmentsFile, appointmentLines);
    }

    // Load all appointments in the system
    public List<Appointment> LoadAllAppointments()
    {
      if (!File.Exists(_appointmentsFile))
        return new List<Appointment>();

      return File.ReadAllLines(_appointmentsFile)
                 .Select(Appointment.FromFileString)
                 .Where(a => a != null)
                 .Cast<Appointment>()
                 .ToList();
    }

    // Load all appointments assigned to one personnel
    public List<Appointment> LoadPersonnelSchedule(int personnelId)
    {
      return LoadAllAppointments()
          .Where(a => a.PersonnelId == personnelId)
          .OrderBy(a => a.Date)
          .ToList();
    }

    // 
    //                             SHIFTS
    // 

    // Load every shift from file
    public List<Shift> LoadAllShifts()
    {
      EnsureDataDirectoryExists();

      if (!File.Exists(_shiftsFile))
        return new List<Shift>();

      return File.ReadAllLines(_shiftsFile)
                 .Select(Shift.FromFileString)
                 .Where(s => s != null)
                 .Cast<Shift>()
                 .ToList();
    }

    // Load all shifts for one personnel
    public List<Shift> LoadShiftsForPersonnel(int personnelId)
    {
      return LoadAllShifts()
          .Where(s => s.PersonnelId == personnelId)
          .OrderBy(s => s.Start)
          .ToList();
    }

    // Save a new shift to file
    public void SaveShift(Shift shift)
    {
      EnsureDataDirectoryExists();

      var shiftLines = File.Exists(_shiftsFile)
          ? File.ReadAllLines(_shiftsFile).ToList()
          : new List<string>();

      shiftLines.Add(shift.ToFileString());
      File.WriteAllLines(_shiftsFile, shiftLines);
    }

    // Find colleagues working during the same shift window
    public List<Shift> GetColleaguesForShift(Shift currentShift)
    {
      return LoadAllShifts()
          .Where(otherShift =>
              otherShift.PersonnelId != currentShift.PersonnelId &&
              (
                  // Overlaps: start inside, end inside, or full overlap
                  (otherShift.Start >= currentShift.Start && otherShift.Start < currentShift.End) ||
                  (otherShift.End > currentShift.Start && otherShift.End <= currentShift.End) ||
                  (otherShift.Start <= currentShift.Start && otherShift.End >= currentShift.End)
              ))
          .OrderBy(s => s.Start)
          .ToList();
    }

    // Ensure data directory exists before saving files
    private void EnsureDataDirectoryExists()
    {
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");
    }

    // 
    //                             SHIFT CLASS
    // 

    public class Shift
    {
      public DateTime Start { get; set; }      // Shift start time
      public DateTime End { get; set; }        // Shift end time
      public int PersonnelId { get; set; }     // Personnel assigned to this shift

      public Shift() { }

      public Shift(DateTime start, DateTime end, int personnelId)
      {
        Start = start;
        End = end;
        PersonnelId = personnelId;
      }

      // Convert shift object to string for saving to file
      public string ToFileString()
      {
        return $"{Start:yyyy-MM-dd HH:mm};{End:yyyy-MM-dd HH:mm};{PersonnelId}";
      }

      // Parse a line from file into a Shift object
      public static Shift? FromFileString(string line)
      {
        if (string.IsNullOrWhiteSpace(line))
          return null;

        var parts = line.Split(';');
        if (parts.Length < 3)
          return null;

        if (!DateTime.TryParse(parts[0], out DateTime start))
          return null;

        if (!DateTime.TryParse(parts[1], out DateTime end))
          return null;

        if (!int.TryParse(parts[2], out int personnelId))
          return null;

        return new Shift(start, end, personnelId);
      }
    }
  }
}
