namespace App

{
  //Represents a single work shift for a staff member
  public class WorkShift
  {
    public int PersonnelId { get; set; } //Which staff member the shift belongs to
    public string PersonnelName { get; set; } //For easy display
    public DateTime Start { get; set; } // Shift start time
    public DateTime End { get; set; } //Shift end time
    public string Department { get; set; } //Where the person works during this shift

    public WorkShift(int personnelId, string personnelName, DateTime start, DateTime end, string department)
    {
      PersonnelId = personnelId;
      PersonnelName = personnelName;
      Start = start;
      End = end;
      Department = department;
    }

    //Used to display in console
    public string Format()
    {
      return $"{PersonnelName,-15} | {Department,-12} | {Start:yyyy-MM-dd HH:mm} - {End:HH:mm}";
    }
  }
}