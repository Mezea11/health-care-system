namespace App
{
  //Represents a single appointment in the healthcare system.
  public class Appointment
  {
    //Properties
    public int UserId {get; set;} //The patient/user ID
    public DateTime Date {get; set;} //Date and time of appointment
    public string Doctor {get; set; } //Doctor responsible
    public string Department {get; set;} //Department
    public string Type {get; set;} //Type of appointment
    public string Status {get; set;} = "Pending"; //Default status
    public int? PersonnelId {get; set;} //Optional assigned personnel
    public bool Is Approved {get; set;} = false; //Approval flag

    public Registartion RegistrationPending {get; set;} = Registration.Pending;

    //Constructors
    public Appointment() {}

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

    //Methods

    //Returns a formatted string for console
    public string Format()
    {
      return $"Date: {Date:yyyy-MM-dd HH:mm} | Doctor: {Doctor,-15} | Department: {Department,-15} | Type: {Type,-12} | Status: {Status}";
    }

    //Converts appointment data to one text line for saving to .txt
    public string ToFileString()
    {
      return $"{UserId};{Date:yyyy-MM-dd HH:mm};{Doctor};{Department};{Type};{Status};{(PersonnelId.HasValue ? PersonnelId.Value.ToString() : "")}";
    }

    //Creates an Appointment object from one line of text
    public static Appointment? FromFileString(string line)
    {
      if (string.IsNullOrWhiteSpace(line))
      return null;

      string[] parts = line.Split(';');
      if (parts.Length < 5)
      return null;

      try
      {
        int userId = int.Parse(parts[0]);
        DateTime date = DateTime.Parse(parts[1]);
        string doctor = parts[2];
        string department = parts[3];
        string type = parts[4];

        var appointment = new Appointment(userId, date, doctor, department, type);

        if(parts.Length > 5)
        appointment.Status = parts[5];
        if (parts.Length > 6 && int.Tryparse(parts[6], out int personnelId))
        appointment.PersonnelId = personnelId;

        return appointment;
      }
      catch
      {
        //Ignore faulty lines
        return null;
      }
    }
  }
}