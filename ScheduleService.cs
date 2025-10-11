namespace App;

//Serive class responsible for reading and managing schedules form a text file.
//This simulates a database by storing appointments in a plain text file.

public class ScheduleService
{
  //Path to the text file that stores all appointments.
  private string _filePath = "Data/schedule.txt";

  ///Loads a schedule for a specific user by reading appointments from the text file.
  /// Only appointments that match the given userId are included in the returned Schedule object.

  /// <param name="userId"> The ID of the user whose schedule should be loaded.</param>
  /// <returns>A Schedule object containing all appointments for the user.</returns>
  public Schedule LoadSchedule(int userId)
  {
    //Cerate a new Schedule object for this user.
    Schedule schedule = new Schedule(userId);

    //Check if the file exists; if not, return an empty schedule
    if (!File.Exists(_filePath))
      return schedule;

    //Read all lines from the file.
    string[] lines = File.ReadAllLines(_filePath);

    foreach (string line in lines)
    {
      //Split the line by semicolon into parts: userId;date;doctor;department;type
      string[] parts = line.Split(';');

      //Skip any lines that do not have enough data.
      if (parts.Length < 5) continue;

      //Parse the userId from the file line
      int fileUserId = int.Parse(parts[0]);

      //Only add appointments that belong to the requested user.
      if (fileUserId == userId)
      {
        DateTime date = DateTime.Parse(parts[1]);
        string doctor = parts[2];
        string department = parts[3];
        string type = parts[4];

        //Create a new Appointment object and add it to the schedule.
        Appointment appointment = new Appointment(fileUserId, date, doctor, department, type);
        schedule.AddAppointment(appointment);
      }
    }
    //Return the schedule with all appointments sorted by date
    return schedule;
  }
  //Daves a new appointment to the the schedule file (simulating a database insert).

  /// <param name="appointment">The appointment to save.</param>
  public void SaveAppointment(Appointment appointment)
  {
    string line = $"{appointment.UserId};{appointment.Date:yyyy-MM-dd HH:mm};{appointment.Department};{appointment.Type}";
    File.AppendAllText(_filePath, line + Environment.NewLine);
  }

}