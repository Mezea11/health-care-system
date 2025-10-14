namespace App;

static class PersonnelUI
{
  static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
  {
    {2, new List<int> {5, 6}},//Example: personnel 2 -> patient 5 and 6
    {3, new List<int> {7}} //personnel 3 -> patient 7
  };
  public static void PersonellMenu(List<IUser> users, IUser activerUser)
  {
    Console.Clear();
    Console.WriteLine($"Personnel Menu - Logged in as {activerUser.Username}\n");

    //Control permissions
    if (activerUser.GetPermissions() != Permissions.AddLocation)
    {
      Console.WriteLine("You do not have permission to access patient journals.");
      Console.ReadKey();
      return;
    }
    //Controll that personnel have assigned patients.
    if (!AssignedPatients.ContainsKey(activerUser.Id) || AssignedPatients[activerUser.Id].Count == 0)
    {
      Console.WriteLine("You are not currently assigned to any patients.");
      Console.ReadKey();
      return;
    }
    var patientIds = AssignedPatients[activerUser.Id];
    var JournalService = new JournalService();

    //List patients
    Console.WriteLine("Assigned Patients: ");
    foreach (int pid in patientIds)
    {
      var patient = users.FirstOrDefault(U => U.Id == pid);
      if (patient != null)
        Console.WriteLine($"- {patient.Username} (ID: {pid})");
    }
    int selectedId = Utils.GetIntegerInput("\nEnter Patient ID to open their journal: ");
    if (!patientIds.Contains(selectedId))
    {
      Console.WriteLine("You are not authortized to access this patient's journal.");
      Console.ReadKey();
      return;
    }
    Console.Clear();
    Console.WriteLine($"--- Journal for Patinet #{selectedId} ---\n");

    var entries = JournalService.GetJournalEntries(selectedId);
    if (entries.Count == 0)
      Console.WriteLine("(No entries yet)");
    else
      entries.ForEach(e => Console.WriteLine(e.Format()));

    Console.WriteLine("\nDo you want to add a new entry? (y/n): ");
    string choice = Console.ReadLine()?.Trim().ToLower() ?? "";

    if (choice == "y")
    {
      Console.WriteLine("Enter new journal text: ");
      string newText = Console.ReadLine() ?? "";
      journalService.AddEntry(selectedId, activerUser.Username, newText);
      Console.WriteLine("Entry added successfully!");
    }
    Console.WriteLine("\nPress any key to return to menu...");
    Console.ReadKey();
  }
}