using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using App;

namespace App
{
  // UI logic for personnel
  public static class PersonnelUI
  {
    public static void ModifyAppointment(List<IUser> users, IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"--- Modify Patient Appointment (Personnel: {activeUser.Username}) ---\n");

      //Mock: Assigned patients
      List<int> assignedPatients = new List<int> { 5, 6 };

      Console.WriteLine("Assigned Patients: ");
      foreach (int pid in assignedPatients)
      {
        var patient = users.FirstOrDefault(u => u.Id == pid);
        if (patient != null)
          Console.WriteLine($"- {patient.Username} (ID: {pid})");
      }
      int selectedPatientId = Utils.GetIntegerInput("\nEnter Patient ID to modify appointments: ");
      if (!assignedPatients.Contains(selectedPatientId))
      {
        Utils.DisplayAlertText("You are not authorized to access this patient's schedule.");
        return;
      }
      var scheduleService = new ScheduleService();
      var schedule = scheduleService.LoadSchedule(selectedPatientId);

      if (schedule.Appointments.Count == 0)
      {
        Utils.DisplayAlertText("No appointments found for this patient.");
        return;
      }
      Console.WriteLine("\nPatient Appointments: ");
      for (int i = 0; i < schedule.Appointments.Count; i++)
      {
        Console.WriteLine($"{i + 1}. {schedule.Appointments[i].Format()}");
      }
      int selectedIndex = Utils.GetIntegerInput("\nChoose appointment number to modify: ") - 1;

      if (selectedIndex < 0 || selectedIndex >= schedule.Appointments.Count)
      {
        Utils.DisplayAlertText("Invalid selection.");
        return;
      }
      var appointment = schedule.Appointments[selectedIndex];

      //Update fields 
      string newDoctor = Utils.GetRequiredInput($"Doctor ({appointment.Doctor}): ");
      string newDepartment = Utils.GetRequiredInput($"Department ({appointment.Department}): ");
      string newType = Utils.GetRequiredInput($"Type ({appointment.Type}): ");
      string dateInput = Utils.GetRequiredInput($"Date & Time ({appointment.Date:yyyy-MM-dd HH:mm}): ");

      if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newDate))
      {
        Utils.DisplayAlertText("Invalid date format. Modification canceled.");
        return;
      }
      appointment.Doctor = newDoctor;
      appointment.Department = newDepartment;
      appointment.Type = newType;
      appointment.Date = newDate;

      //Save updated appointment
      scheduleService.SaveAppointment(appointment);

      Utils.DisplaySuccesText("Appointment modified seccessfully!");
    }
    // Mock dictionary of personnel assignments: personnel ID -> list of patient IDs
    static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
        {
            {2, new List<int> {5, 6}}, // Example: personnel 2 -> patients 5 and 6
            {3, new List<int> {7}}     // Example: personnel 3 -> patient 7
        };

    // =========================================
    // Opens a patient's journal
    // =========================================
    public static void OpenJournal(List<IUser> users, IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"--- Open Journal (Personnel: {activeUser.Username}) ---\n");

      // Check if the personnel has assigned patients
      if (!AssignedPatients.ContainsKey(activeUser.Id) || AssignedPatients[activeUser.Id].Count == 0)
      {
        Utils.DisplayAlertText("You are not currently assigned to any patients.");
        Console.ReadKey();
        return;
      }

      // Get the list of assigned patient IDs
      List<int> assignedPatients = AssignedPatients[activeUser.Id];

      // List assigned patients
      Console.WriteLine("Assigned Patients:");
      foreach (int pid in assignedPatients)
      {
        var patient = users.FirstOrDefault(u => u.Id == pid);
        if (patient != null)
          Console.WriteLine($"- {patient.Username} (ID: {pid})");
      }

      int selectedId = Utils.GetIntegerInput("\nEnter Patient ID to open their journal: ");
      if (!assignedPatients.Contains(selectedId))
      {
        Utils.DisplayAlertText("You are not authorized to access this patient's journal.");
        return;
      }

      // Load journal entries
      var journalService = new JournalService();
      var entries = journalService.GetJournalEntries(selectedId);

      Console.Clear();
      Console.WriteLine($"--- Journal for Patient #{selectedId} ---\n");
      if (entries.Count == 0)
        Console.WriteLine("(No entries yet)");
      else
        entries.ForEach(e => Console.WriteLine(e.Format()));

      // Prompt to add new entry
      Console.WriteLine("\nAdd a new entry? (y/n): ");
      string choice = Console.ReadLine()?.Trim().ToLower() ?? "";
      if (choice == "y")
      {
        string newText = Utils.GetRequiredInput("Enter new journal text: ");
        journalService.AddEntry(selectedId, activeUser.Username, newText);

        // Send notify's
        var notifier = new NotificationService();
        notifier.NotifyPatient(selectedId, $"Your journal has been updated by {activeUser.Username}.");

        Utils.DisplaySuccesText("Entry added successfully!");
      }


      Console.WriteLine("\nPress any key to return to menu...");
      Console.ReadKey();
    }
  }
}
