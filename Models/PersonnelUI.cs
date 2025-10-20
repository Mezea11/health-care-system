using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
  public static class PersonnelUI
  {
    // Mock dictionary of personnel assignments: personnel ID -> list of patient IDs
    static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
    {
        { 2, new List<int> { 5, 6 } },
        { 3, new List<int> { 7 } }
    };

    // =========================================
    // Modify a patient's appointment
    // =========================================
    public static void ModifyAppointment(List<IUser> users, IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"--- Modify Patient Appointment (Personnel: {activeUser.Username}) ---\n");

      if (!AssignedPatients.ContainsKey(activeUser.Id) || AssignedPatients[activeUser.Id].Count == 0)
      {
        Utils.DisplayAlertText("You are not currently assigned to any patients.");
        Console.ReadKey();
        return;
      }

      List<int> assignedPatients = AssignedPatients[activeUser.Id];
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
        Console.WriteLine($"{i + 1}. {schedule.Appointments[i].Format()}");

      int selectedIndex = Utils.GetIntegerInput("\nChoose appointment number to modify: ") - 1;
      if (selectedIndex < 0 || selectedIndex >= schedule.Appointments.Count)
      {
        Utils.DisplayAlertText("Invalid selection.");
        return;
      }

      var appointment = schedule.Appointments[selectedIndex];

      string newDoctor = Utils.GetRequiredInput($"Doctor ({appointment.Doctor}): ");
      string newDepartment = Utils.GetRequiredInput($"Department ({appointment.Department}): ");
      string newType = Utils.GetRequiredInput($"Type ({appointment.Type}): ");
      string dateInput = Utils.GetRequiredInput($"Date & Time ({appointment.Date:yyyy-MM-dd HH:mm}): ");

      if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", null,
          System.Globalization.DateTimeStyles.None, out DateTime newDate))
      {
        Utils.DisplayAlertText("Invalid date format. Modification canceled.");
        return;
      }

      appointment.Doctor = newDoctor;
      appointment.Department = newDepartment;
      appointment.Type = newType;
      appointment.Date = newDate;
      appointment.PersonnelId = activeUser.Id;

      scheduleService.SaveAppointment(appointment);
      Utils.DisplaySuccesText("Appointment modified successfully!");
    }

    // =========================================
    // Open a patient's journal
    // =========================================
    public static void OpenJournal(List<IUser> users, IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"--- Open Journal (Personnel: {activeUser.Username}) ---\n");

      if (!AssignedPatients.ContainsKey(activeUser.Id) || AssignedPatients[activeUser.Id].Count == 0)
      {
        Utils.DisplayAlertText("You are not currently assigned to any patients.");
        Console.ReadKey();
        return;
      }

      List<int> assignedPatients = AssignedPatients[activeUser.Id];
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

      var journalService = new JournalService();
      var entries = journalService.GetJournalEntries(selectedId);

      Console.Clear();
      Console.WriteLine($"--- Journal for Patient #{selectedId} ---\n");
      if (entries.Count == 0)
        Console.WriteLine("(No entries yet)");
      else
        entries.ForEach(e => Console.WriteLine(e.Format()));

      Console.WriteLine("\nAdd a new entry? (y/n): ");
      string choice = Console.ReadLine()?.Trim().ToLower() ?? "";
      if (choice == "y")
      {
        string newText = Utils.GetRequiredInput("Enter new journal text: ");
        journalService.AddEntry(selectedId, activeUser.Username, newText);

        Utils.DisplaySuccesText("Entry added successfully!");
      }

      Console.WriteLine("\nPress any key to return to menu...");
      Console.ReadKey();
    }

    // =========================================
    // Advanced interactive schedule view
    // =========================================
    public static void ViewMySchedule(IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"--- My Work Schedule ({activeUser.Username}) ---\n");

      var scheduleService = new ScheduleService();

      var allAppointments = scheduleService.LoadAllAppointments()
          .Where(a => a.PersonnelId == activeUser.Id)
          .OrderBy(a => a.Date)
          .ToList();

      var shifts = scheduleService.LoadShiftsForPersonnel(activeUser.Id)
          .OrderBy(s => s.Start)
          .ToList();

      Console.WriteLine("Filter by date? (yyyy-MM-dd) or leave empty for all: ");
      string dateFilterInput = Console.ReadLine()?.Trim() ?? "";
      DateTime? filterDate = null;
      if (!string.IsNullOrEmpty(dateFilterInput) && DateTime.TryParse(dateFilterInput, out DateTime parsedDate))
        filterDate = parsedDate.Date;

      foreach (var shift in shifts)
      {
        if (filterDate.HasValue && shift.Start.Date != filterDate.Value)
          continue;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nShift: {shift.Start:yyyy-MM-dd HH:mm} - {shift.End:HH:mm}");
        Console.ResetColor();

        var colleagues = scheduleService.GetColleaguesForShift(shift);
        if (colleagues.Any())
        {
          Console.ForegroundColor = ConsoleColor.DarkCyan;
          Console.WriteLine("👥 Colleagues working this shift:");
          Console.ResetColor();

          foreach (var c in colleagues)
            Console.WriteLine($"   - Personnel ID: {c.PersonnelId} ({c.Start:HH:mm}-{c.End:HH:mm})");
        }
        else
        {
          Console.WriteLine("👤 No colleagues working this shift.");
        }

        var appsInShift = allAppointments
            .Where(a => a.Date >= shift.Start && a.Date < shift.End)
            .OrderBy(a => a.Date)
            .ToList();

        TimeSpan totalShiftTime = shift.End - shift.Start;
        TimeSpan bookedTime = TimeSpan.FromMinutes(appsInShift.Count * 30);
        TimeSpan freeTime = totalShiftTime - bookedTime;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\nShift Summary:");
        Console.ResetColor();
        Console.WriteLine($" Total Shift Time: {totalShiftTime.TotalHours:F1} hours");
        Console.WriteLine($" Appointments: {appsInShift.Count}");
        Console.WriteLine($" Estimated Booked Time: {bookedTime.TotalHours:F1} hours");
        Console.WriteLine($" Free Time: {freeTime.TotalHours:F1} hours");

        if (!appsInShift.Any())
        {
          Console.WriteLine("  (No appointments in this shift)");
          continue;
        }

        Console.WriteLine("\n+----+-------------------+-----------------+-----------------+-----------+");
        Console.WriteLine("| #  | Date & Time       | Patient         | Type            | Status    |");
        Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

        for (int i = 0; i < appsInShift.Count; i++)
        {
          var a = appsInShift[i];
          string patientName = $"Patient {a.UserId}";
          ConsoleColor statusColor = a.Status.ToLower() switch
          {
            "pending" => ConsoleColor.Yellow,
            "confirmed" => ConsoleColor.Green,
            "cancelled" => ConsoleColor.Red,
            _ => ConsoleColor.White
          };

          Console.ForegroundColor = statusColor;
          Console.WriteLine($"| {i + 1,-2} | {a.Date:yyyy-MM-dd HH:mm} | {patientName,-15} | {a.Type,-15} | {a.Status,-9} |");
          Console.ResetColor();
        }

        Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

        bool keepEditing = true;
        while (keepEditing)
        {
          Console.WriteLine("\nSelect appointment number to change status, 'n' for next shift, or 'q' to quit: ");
          string input = Console.ReadLine()?.Trim().ToLower() ?? "";
          if (input == "q") return;
          if (input == "n") break;

          if (int.TryParse(input, out int selectedNum) && selectedNum > 0 && selectedNum <= appsInShift.Count)
          {
            var selectedAppointment = appsInShift[selectedNum - 1];
            Console.WriteLine("Change status to: 1) Pending 2) Confirmed 3) Cancelled");
            string statusChoice = Console.ReadLine()?.Trim() ?? "";
            switch (statusChoice)
            {
              case "1": selectedAppointment.Status = "Pending"; break;
              case "2": selectedAppointment.Status = "Confirmed"; break;
              case "3": selectedAppointment.Status = "Cancelled"; break;
              default: Console.WriteLine("Invalid choice."); continue;
            }
            scheduleService.SaveAppointment(selectedAppointment);
            Utils.DisplaySuccesText("Appointment status updated!");
          }
        }
      }

      Console.WriteLine("\nAll shifts displayed. Press any key to return...");
      Console.ReadKey();
    }
  }
}
