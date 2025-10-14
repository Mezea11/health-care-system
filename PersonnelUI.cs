using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
  public static class PersonnelUI
  {
    // Öppna en patients journal (observera public!)
    public static void OpenJournal(List<IUser> users, IUser activeUser)
    {
      // Example: check permissions, assigned patients
      Console.Clear();
      Console.WriteLine($"--- Open Journal (Personnel: {activeUser.Username}) ---\n");

      // Lista mockade patienter för demo
      List<int> assignedPatients = new List<int> { 5, 6 };

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
  }
}
