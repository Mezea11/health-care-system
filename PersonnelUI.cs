using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
  public static class PersonnelUI
  {
    // AssignedPatients dictionary: personnelId -> list of patientIds
    private static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
        {
            { 2, new List<int> { 5, 6 } }, // Example: personnel 2 -> patient 5 and 6
            { 3, new List<int> { 7 } }     // personnel 3 -> patient 7
        };

    // OpenJournal handles all journal access for personnel
    public static void OpenJournal(List<IUser> users, IUser activeUser)
    {
      Console.Clear();
      Console.WriteLine($"Personnel Menu - Logged in as {activeUser.Username}\n");

      // Check if user has permission to access patient journals
      if (!activeUser.HasPermission("AddLocation")) // <-- adjust permission logic if needed
      {
        Console.WriteLine("You do not have permission to access patient journals.");
        Console.ReadKey();
        return;
      }

      // Check if personnel has assigned patients
      if (!AssignedPatients.ContainsKey(activeUser.Id) || AssignedPatients[activeUser.Id].Count == 0)
      {
        Console.WriteLine("You are not currently assigned to any patients.");
        Console.ReadKey();
        return;
      }

      var patientIds = AssignedPatients[activeUser.Id];
      var journalService = new JournalService();

      // List assigned patients
      Console.WriteLine("Assigned Patients: ");
      foreach (int pid in patientIds)
      {
        var patient = users.FirstOrDefault(u => u.Id == pid);
        if (patient != null)
          Console.WriteLine($"- {patient.Username} (ID: {pid})");
      }

      int selectedId = Utils.GetIntegerInput("\nEnter Patient ID to open their journal: ");

      if (!patientIds.Contains(selectedId))
      {
        Console.WriteLine("You are not authorized to access this patient's journal.");
        Console.ReadKey();
        return;
      }

      Console.Clear();
      Console.WriteLine($"--- Journal for Patient #{selectedId} ---\n");

      var entries = journalService.GetJournalEntries(selectedId);
      if (entries.Count == 0)
        Console.WriteLine("(No entries yet)");
      else
        entries.ForEach(e => Console.WriteLine(e.Format()));

      Console.Write("\nDo you want to add a new entry? (y/n): ");
      string choice = Console.ReadLine()?.Trim().ToLower() ?? "";

      if (choice == "y")
      {
        Console.WriteLine("\nEnter new journal text:");
        string newText = Console.ReadLine() ?? "";
        journalService.AddEntry(selectedId, activeUser.Username, newText);
        Utils.DisplaySuccesText("Entry added successfully!");
      }

      Console.WriteLine("\nPress any key to return to menu...");
      Console.ReadKey();
    }
  }
}
