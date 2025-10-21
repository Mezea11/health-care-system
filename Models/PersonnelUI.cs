using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
  public static class PersonnelUI
  {
    // List over which patients each personnel is assgined to.
    private static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
    {
        { 2, new List<int> { 5, 6 } },
        { 3, new List<int> { 7 } }
    };

  
    // Accept or deny booknig  
    public static void ApproveAppointments(List<IUser> allUsers, IUser loggedInPersonnel)
    {
      Console.Clear();
      Console.WriteLine($"--- Accept or deny booking request. ({loggedInPersonnel.Username}) ---\n");

      var scheduleService = new ScheduleService();
      var allAppointments = scheduleService.LoadAllAppointments();

      // Filter out bookning that are not accepted yet.
      var pendingAppointments = allAppointments.Where(appointment => !appointment.IsApproved).ToList();

      if (pendingAppointments.Count == 0)
      {
        Utils.DisplayAlertText("They are no bookings waitning to be accepted.");
        Console.ReadKey();
        return;
      }

      // Show all pending bookings.
      for (int i = 0; i < pendingAppointments.Count; i++)
      {
        var appointment = pendingAppointments[i];
        var patientUser = allUsers.FirstOrDefault(user => user.Id == appointment.UserId);
        string patientName = patientUser != null ? patientUser.Username : $"Unknown patient ID (ID {appointment.UserId})";
        Console.WriteLine($"{i + 1}. {patientName} - {appointment.Format()}");
      }

      int selectedIndex = Utils.GetIntegerInput("\nChoose bookning to manage (0 to cancel): ");
      if (selectedIndex == 0 || selectedIndex > pendingAppointments.Count)
        return;

      var selectedAppointment = pendingAppointments[selectedIndex - 1];
      Console.WriteLine($"\nChoosed bookning: {selectedAppointment.Format()}");

      Console.WriteLine("Accept (A) or Deny (D)? ");
      string choice = Console.ReadLine()?.Trim().ToLower() ?? "";

      if (choice == "a")
      {
        selectedAppointment.IsApproved = true;
        selectedAppointment.Status = "Confirmed";
        scheduleService.SaveAppointment(selectedAppointment);
        Utils.DisplaySuccesText("The bookning has been approved!");
      }
      else if (choice == "d")
      {
        scheduleService.RemoveAppointment(selectedAppointment.UserId, selectedAppointment.Date);
        Utils.DisplayAlertText("The booking was denied and has been deleted.");
      }
      else
      {
        Utils.DisplayAlertText("Invalid selection. Returns to menu...");
      }

      Console.WriteLine("\nPress any key to return...");
      Console.ReadKey();
    }

    
    // Modify a patients bookning
    
    public static void ModifyAppointment(List<IUser> allUsers, IUser loggedInPersonnel)
    {
      Console.Clear();
      Console.WriteLine($"--- Modify patient booking ({loggedInPersonnel.Username}) ---\n");

      if (!AssignedPatients.ContainsKey(loggedInPersonnel.Id) || AssignedPatients[loggedInPersonnel.Id].Count == 0)
      {
        Utils.DisplayAlertText("You are not connected to any patients at this time..");
        Console.ReadKey();
        return;
      }

      // Show patients that are assigned to a personnel.
      List<int> patientIds = AssignedPatients[loggedInPersonnel.Id];
      Console.WriteLine("Assigned patients: ");
      foreach (int patientId in patientIds)
      {
        var patient = allUsers.FirstOrDefault(user => user.Id == patientId);
        if (patient != null)
          Console.WriteLine($"- {patient.Username} (ID: {patientId})");
      }

      int selectedPatientId = Utils.GetIntegerInput("\nEnter patient ID to view their appointments: ");
      if (!patientIds.Contains(selectedPatientId))
      {
        Utils.DisplayAlertText("You do not have permission to change this patien's bookings.");
        return;
      }

      var scheduleService = new ScheduleService();
      var patientSchedule = scheduleService.LoadSchedule(selectedPatientId);

      if (patientSchedule.Appointments.Count == 0)
      {
        Utils.DisplayAlertText("This patient does not have any appointments.");
        return;
      }

      Console.WriteLine("\nPatients appointments:");
      for (int i = 0; i < patientSchedule.Appointments.Count; i++)
        Console.WriteLine($"{i + 1}. {patientSchedule.Appointments[i].Format()}");

      int selectedIndex = Utils.GetIntegerInput("\nSelect which appointments to change: ") - 1;
      if (selectedIndex < 0 || selectedIndex >= patientSchedule.Appointments.Count)
      {
        Utils.DisplayAlertText("Unvalid choice.");
        return;
      }

      var selectedAppointment = patientSchedule.Appointments[selectedIndex];

      string newDoctor = Utils.GetRequiredInput($"Doctor ({selectedAppointment.Doctor}): ");
      string newDepartment = Utils.GetRequiredInput($"Department ({selectedAppointment.Department}): ");
      string newType = Utils.GetRequiredInput($"Type ({selectedAppointment.Type}): ");
      string newDateInput = Utils.GetRequiredInput($"Date & Time ({selectedAppointment.Date:yyyy-MM-dd HH:mm}): ");

      if (!DateTime.TryParseExact(newDateInput, "yyyy-MM-dd HH:mm", null,
          System.Globalization.DateTimeStyles.None, out DateTime newDate))
      {
        Utils.DisplayAlertText("Unvalid dateformat. The change was canceled.");
        return;
      }

      selectedAppointment.Doctor = newDoctor;
      selectedAppointment.Department = newDepartment;
      selectedAppointment.Type = newType;
      selectedAppointment.Date = newDate;
      selectedAppointment.PersonnelId = loggedInPersonnel.Id;

      scheduleService.SaveAppointment(selectedAppointment);
      Utils.DisplaySuccesText("Appointment have been updated!");
    }

    
    // Open Patient journal
    
    public static void OpenJournal(List<IUser> allUsers, IUser loggedInPersonnel)
    {
      Console.Clear();
      Console.WriteLine($"--- Open patient's journal ({loggedInPersonnel.Username}) ---\n");

      if (!AssignedPatients.ContainsKey(loggedInPersonnel.Id) || AssignedPatients[loggedInPersonnel.Id].Count == 0)
      {
        Utils.DisplayAlertText("You are not assigned to any patient's.");
        Console.ReadKey();
        return;
      }

      List<int> patientIds = AssignedPatients[loggedInPersonnel.Id];
      Console.WriteLine("Assigned patient's:");
      foreach (int patientId in patientIds)
      {
        var patient = allUsers.FirstOrDefault(user => user.Id == patientId);
        if (patient != null)
          Console.WriteLine($"- {patient.Username} (ID: {patientId})");
      }

      int selectedPatientId = Utils.GetIntegerInput("\nSelect patient ID to open journal: ");
      if (!patientIds.Contains(selectedPatientId))
      {
        Utils.DisplayAlertText("You have no permission to view this patient's journal.");
        return;
      }

      var journalService = new JournalService();
      var journalEntries = journalService.GetJournalEntries(selectedPatientId);

      Console.Clear();
      Console.WriteLine($"--- Journal of patient #{selectedPatientId} ---\n");
      if (journalEntries.Count == 0)
        Console.WriteLine("(No entires yet)");
      else
        journalEntries.ForEach(entry => Console.WriteLine(entry.Format()));

      Console.WriteLine("\nDo you want to add an entry? (y/n): ");
      string addNewEntry = Console.ReadLine()?.Trim().ToLower() ?? "";
      if (addNewEntry == "j")
      {
        string newText = Utils.GetRequiredInput("Write your entry: ");
        journalService.AddEntry(selectedPatientId, loggedInPersonnel.Username, newText);
        Utils.DisplaySuccesText("Entry saved!");
      }

      Console.WriteLine("\nPress any key to return...");
      Console.ReadKey();
    }

    
    // Show personnel schedule
    
    public static void ViewMySchedule(IUser loggedInPersonnel)
    {
      Console.Clear();
      Console.WriteLine($"--- My schedule ({loggedInPersonnel.Username}) ---\n");

      var scheduleService = new ScheduleService();
      var allAppointments = scheduleService.LoadAllAppointments()
          .Where(appointment => appointment.PersonnelId == loggedInPersonnel.Id)
          .OrderBy(appointment => appointment.Date)
          .ToList();

      if (!allAppointments.Any())
      {
        Utils.DisplayAlertText("You have no appointment's in your schedule.");
        Console.ReadKey();
        return;
      }

      Console.WriteLine("Your upcoming appointments:\n");
      foreach (var appointment in allAppointments)
      {
        Console.WriteLine(appointment.Format());
      }

      Console.WriteLine("\nPress any key to return...");
      Console.ReadKey();
    }
  }
}
