namespace App
{
 
  // ScheduleService
  // Handles saving and loading of patient appointments and personnel shifts
  // using plain text files (.txt)

  public class ScheduleService
  {
    private readonly string _appointmentsFile = "Data/appointments.txt";
    private readonly string _shiftsFile = "Data/shifts.txt";

  
    // APPOINTMENTS
   

    
    // Load all appointments for a specific user (patient or staff).
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
        {
          schedule.AddAppointment(appointment);
        }
      }

      return schedule;
    }

    // Save or update a single appointment.
    // If an appointment exists for same user/date, it is replaced.
    public void SaveAppointment(Appointment appointment)
    {
      EnsureDataDirectoryExists();

      List<string> appointmentLines = File.Exists(_appointmentsFile)
        ? File.ReadAllLines(_appointmentsFile).ToList()
        : new List<string>();

      // Remove any existing appointment for the same user/date
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

    
    // Remove a specific appointment by user and date.
    public void RemoveAppointment(int userId, DateTime appointmentDate)
    {
      if (!File.Exists(_appointmentsFile)) return;

      var appointmentLines = File.ReadAllLines(_appointmentsFile).ToList();

      appointmentLines.RemoveAll(line =>
      {
        var appointment = Appointment.FromFileString(line);
        return appointment != null &&
               appointment.UserId == userId &&
               appointment.Date == appointmentDate;
      });

      File.WriteAllLines(_appointmentsFile, appointmentLines);
    }

  
    // Load all appointments (used for personnel filtering, approvals, etc.)
    public List<Appointment> LoadAllAppointments()
    {
      if (!File.Exists(_appointmentsFile)) return new List<Appointment>();

      var appointmentLines = File.ReadAllLines(_appointmentsFile);
      var allAppointments = new List<Appointment>();

      foreach (var line in appointmentLines)
      {
        var appointment = Appointment.FromFileString(line);
        if (appointment != null)
          allAppointments.Add(appointment);
      }

      return allAppointments;
    }


    // Load appointments assigned to a specific personnel member.
    public List<Appointment> LoadPersonnelSchedule(int personnelId)
    {
      var allAppointments = LoadAllAppointments();
      return allAppointments
        .Where(appointment => appointment.PersonnelId == personnelId)
        .OrderBy(appointment => appointment.Date)
        .ToList();
    }


    // SHIFTS


    /// Load all shifts from file.
    public List<Shift> LoadAllShifts()
    {
      EnsureDataDirectoryExists();

      if (!File.Exists(_shiftsFile)) return new List<Shift>();

      var shiftLines = File.ReadAllLines(_shiftsFile);
      var allShifts = new List<Shift>();

      foreach (var line in shiftLines)
      {
        var shift = Shift.FromFileString(line);
        if (shift != null) allShifts.Add(shift);
      }

      return allShifts;
    }

    /// Load shifts assigned to a specific personnel member.
    public List<Shift> LoadShiftsForPersonnel(int personnelId)
    {
      var allShifts = LoadAllShifts();
      return allShifts
        .Where(shift => shift.PersonnelId == personnelId)
        .OrderBy(shift => shift.Start)
        .ToList();
    }

    /// Save a single shift.
    public void SaveShift(Shift shift)
    {
      EnsureDataDirectoryExists();

      var shiftLines = File.Exists(_shiftsFile) ? File.ReadAllLines(_shiftsFile).ToList() : new List<string>();
      shiftLines.Add(shift.ToFileString());
      File.WriteAllLines(_shiftsFile, shiftLines);
    }

    /// Get colleagues who have overlapping shifts.
    public List<Shift> GetColleaguesForShift(Shift currentShift)
    {
      var allShifts = LoadAllShifts();

      return allShifts
        .Where(otherShift =>
          otherShift.PersonnelId != currentShift.PersonnelId &&
          ((otherShift.Start >= currentShift.Start && otherShift.Start < currentShift.End) ||
           (otherShift.End > currentShift.Start && otherShift.End <= currentShift.End) ||
           (otherShift.Start <= currentShift.Start && otherShift.End >= currentShift.End)))
        .OrderBy(overlappingShift => overlappingShift.Start)
        .ToList();
    }


    // HELPERS

    private void EnsureDataDirectoryExists()
    {
      if (!Directory.Exists("Data"))
        Directory.CreateDirectory("Data");
    }

    
    // SHIFT CLASS
    
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

      // Convert shift to a line for .txt file storage.
      public string ToFileString()
      {
        return $"{Start:yyyy-MM-dd HH:mm};{End:yyyy-MM-dd HH:mm};{PersonnelId}";
      }

      // Create a shift from a line read from file.
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
