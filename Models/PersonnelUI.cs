
namespace App
{
    // PersonnelUI handles personnel interactions with schedules, appointments, and patient journals
    public static class PersonnelUI
    {
        // Dictionary mapping personnel ID → list of assigned patient IDs
        private static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>();

        // Approve or deny pending appointments
        public static void ApproveAppointments(List<User> allUsers, User activePersonnel)
        {
            Console.Clear();
            Console.WriteLine($"--- Approve/Deny Appointments (Personnel: {activePersonnel.Username}) ---\n");

            var scheduleService = new ScheduleService();
            var allAppointments = scheduleService.LoadAllAppointments();

            var pendingAppointments = allAppointments.Where(appointment => !appointment.IsApproved).ToList();

            if (!pendingAppointments.Any())
            {
                Console.WriteLine("No pending appointments.");
                Console.ReadKey();
                return;
            }

            for (int index = 0; index < pendingAppointments.Count; index++)
            {
                var appointment = pendingAppointments[index];
                string patientName = allUsers.FirstOrDefault(user => user.Id == appointment.UserId)?.Username ?? $"Patient {appointment.UserId}";
                Console.WriteLine($"{index + 1}. {patientName} - {appointment.Format()}");
            }

            int selectedIndex = Utils.GetIntegerInput("\nSelect appointment to review (0 to cancel): ");
            if (selectedIndex <= 0 || selectedIndex > pendingAppointments.Count) return;

            var selectedAppointment = pendingAppointments[selectedIndex - 1];
            Console.WriteLine($"\nSelected: {selectedAppointment.Format()}");
            Console.WriteLine("Approve (A) or Deny (D)?");
            string action = Console.ReadLine()?.Trim().ToLower() ?? "";

            if (action == "a")
            {
                selectedAppointment.IsApproved = true;
                selectedAppointment.Status = "Approved";
                selectedAppointment.PersonnelId = activePersonnel.Id;
                scheduleService.SaveAppointment(selectedAppointment);

                // Assign patient to personnel for journal access
                if (!AssignedPatients.ContainsKey(activePersonnel.Id))
                    AssignedPatients[activePersonnel.Id] = new List<int>();
                if (!AssignedPatients[activePersonnel.Id].Contains(selectedAppointment.UserId))
                    AssignedPatients[activePersonnel.Id].Add(selectedAppointment.UserId);

                Console.WriteLine("Appointment approved and assigned.");
            }
            else if (action == "d")
            {
                scheduleService.RemoveAppointment(selectedAppointment.UserId, selectedAppointment.Date);
                Console.WriteLine("Appointment denied and removed.");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // View the logged-in personnel's own schedule
        public static void ViewMySchedule(List<User> allUsers, User activePersonnel)
        {
            Console.Clear();
            Console.WriteLine($"--- My Work Schedule ({activePersonnel.Username}) ---\n");

            var scheduleService = new ScheduleService();
            var myAppointments = scheduleService.LoadPersonnelSchedule(activePersonnel.Id);
            var myShifts = scheduleService.LoadShiftsForPersonnel(activePersonnel.Id);

            if (!myShifts.Any())
            {
                Console.WriteLine("No shifts found.");
                Console.ReadKey();
                return;
            }

            foreach (var shift in myShifts)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nShift: {shift.Start:yyyy-MM-dd HH:mm} - {shift.End:HH:mm}");
                Console.ResetColor();

                // Get appointments that fall within this shift
                var appointmentsInShift = myAppointments
                    .Where(appointment => appointment.Date >= shift.Start && appointment.Date < shift.End)
                    .OrderBy(appointment => appointment.Date)
                    .ToList();

                // Display table header
                Console.WriteLine("\n+----+-------------------+-----------------+-----------------+-----------+");
                Console.WriteLine("| #  | Date & Time       | Patient         | Type            | Status    |");
                Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

                for (int i = 0; i < appointmentsInShift.Count; i++)
                {
                    var appointment = appointmentsInShift[i];
                    string patientName = allUsers.FirstOrDefault(user => user.Id == appointment.UserId)?.Username ?? $"Patient {appointment.UserId}";

                    ConsoleColor statusColor = appointment.Status.ToLower() switch
                    {
                        "pending" => ConsoleColor.Yellow,
                        "approved" => ConsoleColor.Green,
                        "cancelled" => ConsoleColor.Red,
                        _ => ConsoleColor.White
                    };

                    Console.ForegroundColor = statusColor;
                    Console.WriteLine($"| {i + 1,-2} | {appointment.Date:yyyy-MM-dd HH:mm} | {patientName,-15} | {appointment.Type,-15} | {appointment.Status,-9} |");
                    Console.ResetColor();
                }

                Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

                Console.WriteLine("\nPress any key to continue to next shift...");
                Console.ReadKey();
            }

            Console.WriteLine("\nAll shifts displayed. Press any key to return...");
            Console.ReadKey();
        }

        // Open assigned patient's journal for personnel
        public static void OpenJournal(List<User> allUsers, User activePersonnel)
        {
            if (!AssignedPatients.ContainsKey(activePersonnel.Id) || !AssignedPatients[activePersonnel.Id].Any())
            {
                Console.WriteLine("No assigned patients.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Assigned Patients:");
            foreach (int patientId in AssignedPatients[activePersonnel.Id])
            {
                var patient = allUsers.FirstOrDefault(user => user.Id == patientId);
                if (patient != null) Console.WriteLine($"- {patient.Username} (ID: {patientId})");
            }

            int selectedPatientId = Utils.GetIntegerInput("\nEnter Patient ID to view journal: ");
            if (!AssignedPatients[activePersonnel.Id].Contains(selectedPatientId))
            {
                Console.WriteLine("Not authorized to access this journal.");
                Console.ReadKey();
                return;
            }

            var journalService = new JournalService();
            var patientJournalEntries = journalService.GetJournalEntries(selectedPatientId);

            Console.Clear();
            Console.WriteLine($"--- Journal for Patient {selectedPatientId} ---\n");

            if (!patientJournalEntries.Any())
                Console.WriteLine("(No entries yet)");
            else
                foreach (var journalEntry in patientJournalEntries.OrderBy(entry => entry.CreatedAt))
                    Console.WriteLine(journalEntry.Format());

            Console.WriteLine("\nAdd new entry? (y/n): ");
            string addEntryChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (addEntryChoice == "y")
            {
                string newEntryText = Utils.GetRequiredInput("Enter journal text: ");
                journalService.AddEntry(selectedPatientId, activePersonnel.Username, newEntryText);
                Console.WriteLine("Entry added successfully!");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }

        // Modify appointments for assigned patients
        public static void ModifyAppointment(List<User> allUsers, User activePersonnel)
        {
            if (!AssignedPatients.ContainsKey(activePersonnel.Id) || !AssignedPatients[activePersonnel.Id].Any())
            {
                Console.WriteLine("You are not assigned to any patients.");
                Console.ReadKey();
                return;
            }

            var scheduleService = new ScheduleService();

            Console.WriteLine("Assigned Patients:");
            foreach (var patientId in AssignedPatients[activePersonnel.Id])
            {
                var patient = allUsers.FirstOrDefault(user => user.Id == patientId);
                if (patient != null) Console.WriteLine($"- {patient.Username} (ID: {patientId})");
            }

            int selectedPatientId = Utils.GetIntegerInput("\nEnter Patient ID to modify appointments: ");
            if (!AssignedPatients[activePersonnel.Id].Contains(selectedPatientId))
            {
                Console.WriteLine("Not authorized to modify this patient.");
                Console.ReadKey();
                return;
            }

            var patientSchedule = scheduleService.LoadSchedule(selectedPatientId);

            if (!patientSchedule.Appointments.Any())
            {
                Console.WriteLine("No appointments found for this patient.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nPatient Appointments:");
            for (int i = 0; i < patientSchedule.Appointments.Count; i++)
                Console.WriteLine($"{i + 1}. {patientSchedule.Appointments[i].Format()}");

            int selectedIndex = Utils.GetIntegerInput("\nSelect appointment number to modify: ") - 1;
            if (selectedIndex < 0 || selectedIndex >= patientSchedule.Appointments.Count)
            {
                Console.WriteLine("Invalid selection.");
                Console.ReadKey();
                return;
            }

            var selectedAppointment = patientSchedule.Appointments[selectedIndex];

            string newDoctorName = Utils.GetRequiredInput($"Doctor ({selectedAppointment.Doctor}): ");
            string newDepartmentName = Utils.GetRequiredInput($"Department ({selectedAppointment.Department}): ");
            string newAppointmentType = Utils.GetRequiredInput($"Type ({selectedAppointment.Type}): ");
            string newDateInput = Utils.GetRequiredInput($"Date & Time ({selectedAppointment.Date:yyyy-MM-dd HH:mm}): ");

            if (!DateTime.TryParseExact(newDateInput, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newDate))
            {
                Console.WriteLine("Invalid date format. Cancelled.");
                Console.ReadKey();
                return;
            }

            selectedAppointment.Doctor = newDoctorName;
            selectedAppointment.Department = newDepartmentName;
            selectedAppointment.Type = newAppointmentType;
            selectedAppointment.Date = newDate;
            selectedAppointment.PersonnelId = activePersonnel.Id;

            scheduleService.SaveAppointment(selectedAppointment);
            Console.WriteLine("Appointment modified successfully!");
            Console.ReadKey();
        }
    }
}