namespace App
{
  public class NotificationService
  {
    public void NotifyPatient(int patientId, string message)
    {
      //Mock notification
      Console.WriteLine($"[Notification to Patient #{patientId}]: {message}");
    }
  }
}