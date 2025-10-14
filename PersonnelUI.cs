using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
  // UI logic for personnel
  public static class PersonnelUI
  {
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
        Utils.DisplaySuccesText("Entry added successfully!");
      }

      Console.WriteLine("\nPress any key to return to menu...");
      Console.ReadKey();
    }
  }
}
