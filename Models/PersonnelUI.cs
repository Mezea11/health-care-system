namespace App
{
    // PersonnelUI
    // Handles personnel interactions with schedules, appointments, and journals

    public static class PersonnelUI
    {
        // Mock dictionary of personnel assignments: personnel ID -> list of assigned patient IDs
        static Dictionary<int, List<int>> AssignedPatients = new Dictionary<int, List<int>>()
        {
            { 2, new List<int> { 5, 6 } }, // Example: Personnel with ID 2 is responsible for aptients 5 and 6
            { 3, new List<int> { 7 } } // Example: Personnel with ID 3 is responsible for patient 7
        };


        
        // Method to allow personnel to approve or deny appointment requests
        // users - list of all users in the system
        // activePersonnel - the currently logged-in personnel performing the action
        public static void ApproveAppointments(List<IUser> users, IUser activePersonnel)
        {
            Console.Clear(); // Clear the console for fresh view
            Console.WriteLine($"--- Approve or Deny Appointment Requests (Personnel: {activePersonnel.Username}) ---\n");// Display header with name of active personnel

            var scheduleService = new ScheduleService(); // Create a ScheduleService instance to access appointments
            var allAppointments = scheduleService.LoadAllAppointments(); // Load all appointments in the system

            // Filter only pending appointments
            var pendingAppointments = new List<Appointment>(); //Create a list to store appointments that are not yet approved
            foreach (var appointment in allAppointments) // Loop through all appointments and select only those that are still pending
            {
                if (!appointment.IsApproved) // If the appointment is not approved yet
                    pendingAppointments.Add(appointment); // Add it to the pending list
            }

            if (pendingAppointments.Count == 0) // If there are no pending appointments, notify the user and return
            {
                Utils.DisplayAlertText("There are no pending appointments."); // Inform the personnel
                Console.ReadKey(); // Wait for key press so the user can see the message
                return; // Exit the method since tehre is nothing to approve
            }

            // Display all pending appointments with corresponding patient names
            for (int index = 0; index < pendingAppointments.Count; index++)
            {
                var appointment = pendingAppointments[index];

                //Find the patient associated with this appointment
                IUser? patient = null;
                foreach (var user in users)
                {
                    if (user.Id == appointment.UserId)
                    {
                        patient = user;
                        break; // Stop searching once the patient is found
                    }
                }

                // If patient is found, display their username; otherwise show "Unknown"
                string patientName = patient != null ? patient.Username : $"Unknown (ID {appointment.UserId})";

                //Print appointment information with a number for selection
                Console.WriteLine($"{index + 1}. {patientName} - {appointment.Format()}");
            }

            int selectedIndex = Utils.GetIntegerInput("\nSelect appointment to review (0 to cancel): "); // Ask the personnel to select an appointment to review
            if (selectedIndex == 0 || selectedIndex > pendingAppointments.Count) return; // Cancel if 0 i selected or the selection is out of range

            var selectedAppointment = pendingAppointments[selectedIndex - 1]; // Get the appointment that was selected

            Console.WriteLine($"\nSelected: {selectedAppointment.Format()}"); // Show the details of the selected appointment
            Console.WriteLine("Approve (A) or Deny (D)?");

            string action = Console.ReadLine()?.Trim().ToLower() ?? ""; // Read personnel action choice (A = approve, D = deny)

            if (action == "a") // Perform action based on user input
            {
                // Approve the appointment and save changes
                selectedAppointment.IsApproved = true;
                scheduleService.SaveAppointment(selectedAppointment);
                Utils.DisplaySuccesText("Appointment approved!");
            }
            else if (action == "d")
            {
                // Deny the appointment and remove it from schedule
                scheduleService.RemoveAppointment(selectedAppointment.UserId, selectedAppointment.Date);
                Utils.DisplayAlertText("Appointment denied and removed.");
            }
            else
            {
                Utils.DisplayAlertText("Invalid choice. Returning to menu..."); // Pause to let the personnel read the message before returning
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
        }


        // Modify a Patient's Appointment

        public static void ModifyAppointment(List<IUser> users, IUser activePersonnel)
        {
            Console.Clear(); // Clear the console and display header
            Console.WriteLine($"--- Modify Patient Appointment (Personnel: {activePersonnel.Username}) ---\n");

            if (!AssignedPatients.ContainsKey(activePersonnel.Id) || AssignedPatients[activePersonnel.Id].Count == 0) // Check if the active personnel has any assigned patients
            {
                Utils.DisplayAlertText("You are not currently assigned to any patients.");
                Console.ReadKey();
                return; // Exit if no assigned patients
            }

            List<int> assignedPatientIds = AssignedPatients[activePersonnel.Id]; // Get the list of patient IDs assigned to this personnel
            Console.WriteLine("Assigned Patients:"); //Display all assgined patients with theri names
            foreach (int patientId in assignedPatientIds)
            {
                IUser? patient = null;
                foreach (var user in users) // Find the user object that matches this patient ID
                {
                    if (user.Id == patientId)
                    {
                        patient = user;
                        break; // stop searching once the patient is found
                    }
                }

                if (patient != null) //Print patient name and ID if found
                    Console.WriteLine($"- {patient.Username} (ID: {patientId})");
            }

            int selectedPatientId = Utils.GetIntegerInput("\nEnter Patient ID to modify appointments: "); // Ask personnel to select a patient ID to modify appointments for
            if (!assignedPatientIds.Contains(selectedPatientId)) // Ensure the selected patient is assigned to this personnel
            {
                Utils.DisplayAlertText("You are not authorized to access this patient's schedule.");
                return;
            }

            var scheduleService = new ScheduleService(); // Load the slected patient's schedule
            var patientSchedule = scheduleService.LoadSchedule(selectedPatientId);

            if (patientSchedule.Appointments.Count == 0) // Check if the aptient has any appointments
            {
                Utils.DisplayAlertText("No appointments found for this patient.");
                return;
            }

            Console.WriteLine("\nPatient Appointments:"); // Display all appointments for the selected patient
            for (int index = 0; index < patientSchedule.Appointments.Count; index++)
            {
                var appointment = patientSchedule.Appointments[index];
                Console.WriteLine($"{index + 1}. {appointment.Format()}"); // Show formatted appointment
            }

            int selectedAppointmentIndex = Utils.GetIntegerInput("\nChoose appointment number to modify: ") - 1; //Ask personnel to selected an appointment to modify
            if (selectedAppointmentIndex < 0 || selectedAppointmentIndex >= patientSchedule.Appointments.Count) // Validate the selected appointment index
            {
                Utils.DisplayAlertText("Invalid selection.");
                return;
            }

            var appointmentToModify = patientSchedule.Appointments[selectedAppointmentIndex];

            // Prompt personnel for new values, prefilled with existing values
            string newDoctorName = Utils.GetRequiredInput($"Doctor ({appointmentToModify.Doctor}): ");
            string newDepartmentName = Utils.GetRequiredInput($"Department ({appointmentToModify.Department}): ");
            string newAppointmentType = Utils.GetRequiredInput($"Type ({appointmentToModify.Type}): ");
            string newDateInput = Utils.GetRequiredInput($"Date & Time ({appointmentToModify.Date:yyyy-MM-dd HH:mm}): ");

            if (!DateTime.TryParseExact(newDateInput, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newDate)) // Parse the new date input
            {
                Utils.DisplayAlertText("Invalid date format. Modification canceled.");
                return; // Exit if date is invalid
            }

            // Update the appointment with the new values
            appointmentToModify.Doctor = newDoctorName;
            appointmentToModify.Department = newDepartmentName;
            appointmentToModify.Type = newAppointmentType;
            appointmentToModify.Date = newDate;
            appointmentToModify.PersonnelId = activePersonnel.Id; // Assign current personnel as responsible

            // Save the modified appointment
            scheduleService.SaveAppointment(appointmentToModify);
            Utils.DisplaySuccesText("Appointment modified successfully!");
        }


        // Open a Patient's Journal

        public static void OpenJournal(List<IUser> users, IUser activePersonnel)
        {
            Console.Clear(); // Clear the console and show header
            Console.WriteLine($"--- Open Journal (Personnel: {activePersonnel.Username}) ---\n");

            if (!AssignedPatients.ContainsKey(activePersonnel.Id) || AssignedPatients[activePersonnel.Id].Count == 0) // Check if the personnel has any assigned patients
            {
                Utils.DisplayAlertText("You are not currently assigned to any patients.");
                Console.ReadKey();
                return; // Exit if no assigned patients
            }

            List<int> assignedPatientIds = AssignedPatients[activePersonnel.Id]; // Get the list of patient IDs assigned to this personnel
            Console.WriteLine("Assigned Patients:"); // Display all assigned patients with their names
            foreach (int patientId in assignedPatientIds)
            {
                IUser? patient = null;
                foreach (var user in users) // Find the corresponding user object for each patient ID
                {
                    if (user.Id == patientId)
                    {
                        patient = user;
                        break; // Stop searching when found
                    }
                }

                if (patient != null) // Print the patient's name and if found
                    Console.WriteLine($"- {patient.Username} (ID: {patientId})");
            }

            int selectedPatientId = Utils.GetIntegerInput("\nEnter Patient ID to open their journal: "); // Ask personnel to select a patient ID to view the journal
            if (!assignedPatientIds.Contains(selectedPatientId)) // Ensure the selected patient is assigned to this personnel
            {
                Utils.DisplayAlertText("You are not authorized to access this patient's journal.");
                return;
            }

            // Load the patient's journal entries
            var journalService = new JournalService();
            var patientJournalEntries = journalService.GetJournalEntries(selectedPatientId);

            Console.Clear(); // Clear console and show the jounral header
            Console.WriteLine($"--- Journal for Patient #{selectedPatientId} ---\n");
            if (patientJournalEntries.Count == 0) // Display the jounral entries or a message if none exist 
            {
                Console.WriteLine("(No entries yet)");
            }
            else
            {
                foreach (var journalEntry in patientJournalEntries)
                    Console.WriteLine(journalEntry.Format()); // Show each entry in a readable format
            }

            Console.WriteLine("\nAdd a new entry? (y/n): "); //Promt to add a new journal entry
            string addEntryChoice = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (addEntryChoice == "y")
            {
                string newEntryText = Utils.GetRequiredInput("Enter new journal text: "); // Get the text for the new journal entry
                journalService.AddEntry(selectedPatientId, activePersonnel.Username, newEntryText); // Add the new entry to the patient's jounral
                Utils.DisplaySuccesText("Entry added successfully!"); // Notify the user that the entry was added successfully
            }

            Console.WriteLine("\nPress any key to return to menu..."); // Wait for the user to press a key before returning
            Console.ReadKey();
        }

        // View Own Schedule

        public static void ViewMySchedule(IUser activePersonnel)
        {
            Console.Clear(); // Clear console and display header
            Console.WriteLine($"--- My Work Schedule ({activePersonnel.Username}) ---\n");

            //Load all appointments assigned to the active personnel and sort by date
            var scheduleService = new ScheduleService();
            var allAppointments = scheduleService.LoadAllAppointments()
                .Where(appointment => appointment.PersonnelId == activePersonnel.Id)
                .OrderBy(appointment => appointment.Date)
                .ToList();

            // Laod all shifts for the personnel and sort by start time
            var allShifts = scheduleService.LoadShiftsForPersonnel(activePersonnel.Id)
                .OrderBy(shift => shift.Start)
                .ToList();

            // Ask user if they want to filter shifts by a specific date
            Console.WriteLine("Filter by date? (yyyy-MM-dd) or leave empty for all: ");
            string dateFilterInput = Console.ReadLine()?.Trim() ?? "";
            DateTime? filterDate = null;
            if (!string.IsNullOrEmpty(dateFilterInput) && DateTime.TryParse(dateFilterInput, out DateTime parsedDate))
                filterDate = parsedDate.Date;

            foreach (var shift in allShifts) //Iterate through each shift
            {
                if (filterDate.HasValue && shift.Start.Date != filterDate.Value) // Skip shifts that do not match the filter date
                    continue;

                // Display shift start and end time
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nShift: {shift.Start:yyyy-MM-dd HH:mm} - {shift.End:HH:mm}");
                Console.ResetColor();

                var colleagues = scheduleService.GetColleaguesForShift(shift); // Find colleagues working during this shift
                if (colleagues.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("Colleagues working this shift:");
                    Console.ResetColor();

                    foreach (var colleagueShift in colleagues)
                        Console.WriteLine($"   - Personnel ID: {colleagueShift.PersonnelId} ({colleagueShift.Start:HH:mm}-{colleagueShift.End:HH:mm})");
                }
                else
                {
                    Console.WriteLine("No colleagues working this shift.");
                }

                // Get appointmets that fall within this shift
                var appointmentsInShift = allAppointments
                    .Where(appointment => appointment.Date >= shift.Start && appointment.Date < shift.End)
                    .OrderBy(appointment => appointment.Date)
                    .ToList();

                // Calculate shift time statistics
                TimeSpan totalShiftTime = shift.End - shift.Start;
                TimeSpan bookedTime = TimeSpan.FromMinutes(appointmentsInShift.Count * 30);
                TimeSpan freeTime = totalShiftTime - bookedTime;

                // Display shift summary
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\nShift Summary:");
                Console.ResetColor();
                Console.WriteLine($" Total Shift Time: {totalShiftTime.TotalHours:F1} hours");
                Console.WriteLine($" Appointments: {appointmentsInShift.Count}");
                Console.WriteLine($" Estimated Booked Time: {bookedTime.TotalHours:F1} hours");
                Console.WriteLine($" Free Time: {freeTime.TotalHours:F1} hours");

                if (appointmentsInShift.Count == 0)
                {
                    Console.WriteLine("  (No appointments in this shift)");
                    continue; // Skip table display if no appointments
                }

                // Display table header for appointments
                Console.WriteLine("\n+----+-------------------+-----------------+-----------------+-----------+");
                Console.WriteLine("| #  | Date & Time       | Patient         | Type            | Status    |");
                Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

                for (int i = 0; i < appointmentsInShift.Count; i++) // List all appointments in the shift
                {
                    var appointment = appointmentsInShift[i];
                    string patientName = $"Patient {appointment.UserId}";
                    ConsoleColor statusColor = ConsoleColor.White;
                    switch (appointment.Status.ToLower()) // Set color based on appointment status
                    {
                        case "pending": statusColor = ConsoleColor.Yellow; break;
                        case "confirmed": statusColor = ConsoleColor.Green; break;
                        case "cancelled": statusColor = ConsoleColor.Red; break;
                    }

                    Console.ForegroundColor = statusColor; // Display appointment row in the table
                    Console.WriteLine($"| {i + 1,-2} | {appointment.Date:yyyy-MM-dd HH:mm} | {patientName,-15} | {appointment.Type,-15} | {appointment.Status,-9} |");
                    Console.ResetColor();
                }

                Console.WriteLine("+----+-------------------+-----------------+-----------------+-----------+");

                bool keepEditing = true; //Allow user to update appointment statuses interactivelvy
                while (keepEditing)
                {
                    Console.WriteLine("\nSelect appointment number to change status, 'n' for next shift, or 'q' to quit: ");
                    string input = Console.ReadLine()?.Trim().ToLower() ?? "";
                    if (input == "q") return; // Exit entrie view
                    if (input == "n") break; // Go to next shift

                    // Validate input and update selected appointment status
                    if (int.TryParse(input, out int selectedNum) && selectedNum > 0 && selectedNum <= appointmentsInShift.Count)
                    {
                        var selectedAppointment = appointmentsInShift[selectedNum - 1];
                        Console.WriteLine("Change status to: 1) Pending 2) Confirmed 3) Cancelled");
                        string statusChoice = Console.ReadLine()?.Trim() ?? "";
                        switch (statusChoice)
                        {
                            case "1": selectedAppointment.Status = "Pending"; break;
                            case "2": selectedAppointment.Status = "Confirmed"; break;
                            case "3": selectedAppointment.Status = "Cancelled"; break;
                            default: Console.WriteLine("Invalid choice."); continue;
                        }
                        scheduleService.SaveAppointment(selectedAppointment); // Save the updated appointment
                        Utils.DisplaySuccesText("Appointment status updated!");
                    }
                }
            }

            Console.WriteLine("\nAll shifts displayed. Press any key to return..."); // Indicate end of all shifts and wait for user input
            Console.ReadKey();
        }
    }
}
