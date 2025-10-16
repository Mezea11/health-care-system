using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using App;
/* 

-- SAVE AND MOCK DATA TO TEXT FILES --

As a user, I need to be able to log in. CHECKED
As a user, I need to be able to log out. CHECKED
As a user, I need to be able to REQUEST registration as a patient. CHECKED 


As an admin with sufficient permissions, I need to be able to give admins the permission to handle the permission system, in fine granularity.
As an admin with sufficient permissions, I need to be able to assign admins to certain regions.
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations. ??? Doubles ?? 
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations. CHECKED
As an admin with sufficient permissions, I need to be able to give admins the permission to add locations. CHECKED
As an admin with sufficient permissions, I need to be able to give admins the permission to create accounts for personnel. WIP
As an admin with sufficient permissions, I need to be able to give admins the permission to view a list of who has permission to what. 
As an admin with sufficient permissions, I need to be able to add locations.

As an admin with sufficient permissions, I need to be able to accept user registration as patients. -- CHECKED
As an admin with sufficient permissions, I need to be able to deny user registration as patients. -- CHECKED
As an admin with sufficient permissions, I need to be able to create accounts for personnel. CHECKED

As an admin with sufficient permissions, I need to be able to view a list of who has permission to what. 


As personnel with sufficient permissions, I need to be able to view a patients journal entries. CHECKED
As personnel with sufficient permissions, I need to be able to mark journal entries with different levels of read permissions.
As personnel with sufficient permissions, I need to be able to register appointments.
As personnel with sufficient permissions, I need to be able to modify appointments.
As personnel with sufficient permissions, I need to be able to approve appointment requests.
As personnel with sufficient permissions, I need to be able to view the schedule of a location.


As a patient, I need to be able to view my own journal. CHECKED
As a patient, I need to be able to request an appointment. CHECKED
As a logged in user, I need to be able to view my schedule.
*/



static int GetIndexAddOne(List<IUser> users)
{
    return users.Count + 1;
}
// ============================
// Main program
// ============================
List<Location> locations = new List<Location>();
List<Appointment> appointments = new List<Appointment>();
locations.Add(new Location("Skåne", "Lunds Universitetssjukhus"));
locations.Add(new Location("Stockholm", "Karolinska institutet"));

// Lista med alla användare 
List<IUser> users = FileHandler.LoadUsersFromJson();


IUser? activeUser = null;
bool running = true;


StartMenu(users);

// ============================
// START MENU METHOD
// ============================

// THIS METHOD ALLOWS USER TO EITHER LOGIN OR REGISTER
void StartMenu(List<IUser> users)
{
    while (true)
    {

        Console.WriteLine("Welcome");
        Console.WriteLine("1. Request Registration (Patient)");
        Console.WriteLine($"2. Request Registration (Admin)");
        Console.WriteLine("3. Log In");
        // Console.Write("Choice: ");
        // string choice = Console.ReadLine();

        switch (Utils.GetIntegerInput("Pick a number: "))
        {
            case 1:
                // CREATE LOGIC IN HERE TO REGISTER A NEW USER
                string newUser = Utils.GetRequiredInput("Type in your username: "); // PROMPT USER TO INSERT USERNAME
                Console.Clear();

                Console.WriteLine("Type in your password"); // PROMPT USER TO INSERT PASSWORD
                string newPass = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Request Sent.");
                users.Add(new User(Utils.GetIndexAddOne(users), newUser, newPass, Role.Patient));  // CREATE NEW OBJECT (WITH ROLE PATIENT) WITH USERNAME AND PASSWORD
                FileHandler.SaveUsersToJson(users);
                break;
            case 2:
                string newAdmin = Utils.GetRequiredInput("Type in your username: "); // PROMPT USER TO INSERT USERNAME
                Console.Clear();

                Console.WriteLine("Type in your password"); // PROMPT USER TO INSERT PASSWORD
                string newAdminPass = Console.ReadLine() ?? "".Trim();
                Console.Clear();

                Console.WriteLine("Request Sent.");
                users.Add(new User(GetIndexAddOne(users), newAdmin, newAdminPass, Role.Admin));
<<<<<<< HEAD
=======
                FileHandler.SaveUsersToJson(users);
>>>>>>> master
                break;

            case 3:
                MainMenu();
                break;
        }
    }

}
//COMMON METHOD - Show current user's schedule
static void ShowSchedule(IUser activeUser)
{
    Console.Clear();
    Console.WriteLine($"--- Schedule for {activeUser.Username} ---\n");

    var scheduleService = new ScheduleService();
    var schedule = scheduleService.LoadSchedule(activeUser.Id);

    if (schedule.Appointments.Count == 0)
    {
        Utils.DisplayAlertText("No appointments found in your schedule.");
    }
    else
    {
        schedule.PrintSchedule();
    }
    Console.WriteLine("\nPress any key to return...");
    Console.ReadKey();
}

void MainMenu()
{

    // Wrap main program in a function
    // Main program loop
    while (running)
    {
        Console.Clear();
        // ============================
        // MENU: Registration 
        // ============================

        if (activeUser == null)
        {
            // ============================
            // MENU: LOGIN
            // ============================
            Console.WriteLine("--- Health Care System ---");
            string username = Utils.GetRequiredInput("Username: ");
            string password = Utils.GetRequiredInput("Password: ");

            // TryLogin method invoked
            foreach (IUser user in users)
            {
                if (user.TryLogin(username, password))
                {
                    activeUser = user;
                    break;
                }
            }

            if (activeUser == null)
            {
                Console.WriteLine("Wrong login credentials. Press enter to try again.");
                Console.ReadLine();
            }

        }

        else
        {
            // If user as a role as patient has a registratin not yet handled by admin. Will not be able to login. But it will have a active_user as not null
            if (activeUser.GetRegistration() != Registration.Accepted)
            {
                Utils.DisplayAlertText("Your account is still pending. Need to wait for the admin to accept it, Press ENTER to continue");
                Console.ReadKey();
                activeUser = null;
                break;
            }
            // ============================
            // MENU: LOGGED IN
            // ============================
            Console.Clear();
            Console.WriteLine($"Logged in as:  {activeUser.Username} ({activeUser.GetRole()})");
            Console.WriteLine("----------------------------------");
            Console.WriteLine(activeUser.GetRole());
            switch (activeUser.GetRole())
            {

                // ADMIN MENU
                case Role.Admin:
                    AdminMenu(users, locations, activeUser);
                    break;

                // PERSONNEL MENU
                case Role.Personnel:
                    PersonnelMenu(users, activeUser, appointments);
                    break;

                // PATIENT MENU
                case Role.Patient:
                    PatientMenu(activeUser);
                    break;

                // SUPERADMIN MENU
                case Role.SuperAdmin:
                    SuperAdminMenu(users, locations, activeUser);
                    break;

            }
            Console.WriteLine("\nWrite 'logout' or press Enter to continue.");
            string input = Console.ReadLine() ?? "".Trim();
            if (input == "logout") activeUser = null;
        }
    }
}





// ============================
// SUPERADMIN MENU METHOD
// ============================
static void SuperAdminMenu(List<IUser> users, List<Location> locations, IUser activeUser)
{
    Console.WriteLine("\n(SuperAdmin) Options:");
    Console.WriteLine("1. Grant admin to add location");
    Console.WriteLine("2. Grant admin to handle registrations");
    Console.WriteLine("3. Grant admin to create personel");
    Console.WriteLine("4. Grant admin to check list of user permissions");
    Console.WriteLine($"5. See pending admin registration requests");
    Console.WriteLine("6. View my schedule");
    Console.WriteLine($"7. Logout");

    int input = Utils.GetIntegerInput("Chose a number: ");

    switch (input)
    {
        case 1:
            {
                Console.WriteLine("A list of all admins");

                foreach (User user in users.Where(user => user.GetRole() == Role.Admin))
                // foreach(User user in users)
                {
                    // if(user.GetRole() == Role.Admin && user.checkpermissions() == Permissions.None)
                    Console.WriteLine($"{user.ToString()}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string adminName = Utils.GetRequiredInput("Pick admin name you want to handle:  ");
                IUser? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (adminUser != null)
                {
                    string acceptOrDeny = Utils.GetRequiredInput($"You chose: {adminUser.Username}, Do you want accept(y) or deny(d) the permission for adding location?");
                    switch (acceptOrDeny)
                    {
                        case "y":
                            adminUser.GrantPermission(Permissions.AddLocation); // <-- anropa metoden
                            Utils.DisplaySuccesText($"You have accepted the permission add a location to admin user: {adminName}");
                            break;

                        case "d":
                            adminUser.RevokePermission(Permissions.AddLocation);   // <-- anropa metoden
                            Utils.DisplaySuccesText("You have denied the permission");
                            break;
                        default:
                            Utils.DisplayAlertText("Only y or n is handled");
                            break;
                    }
                }
                else
                {
                    Utils.DisplayAlertText("No admin with that name.");
                }
            }
            FileHandler.SaveUsersToJson(users);
            break;
        case 2:
            {
                Console.WriteLine("A list of all admins");

                foreach (User user in users.Where(user => user.GetRole() == Role.Admin))
                // foreach(User user in users)
                {
                    // if(user.GetRole() == Role.Admin && user.checkpermissions() == Permissions.None)
                    Console.WriteLine($"{user.ToString()}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string adminName = Utils.GetRequiredInput("Pick admin name you want to handle:  ");
                IUser? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (adminUser != null)
                {
                    string acceptOrDeny = Utils.GetRequiredInput($"You chose: {adminUser.Username}, Do you want accept(y) or deny(d) the permission for handling registration requests?");
                    switch (acceptOrDeny)
                    {
                        case "y":
                            adminUser.GrantPermission(Permissions.AddRegistrations); // <-- anropa metoden, lägg till permission till listan över permission för den admin
                            Utils.DisplaySuccesText($"You have accepted the permission handle registrations for admin: {adminName} ");
                            break;

                        case "d":
                            adminUser.RevokePermission(Permissions.AddRegistrations);   // <-- anropa metoden, ta bort permissions i listan över permissions. Om listan är tom sätt permissions till None. 
                            Utils.DisplaySuccesText($"You have denied permission handle registrations for user: {adminName} ");
                            break;
                        default:
                            Utils.DisplayAlertText("Only y or n is handled");
                            break;
                    }
                }
                else
                {
                    Utils.DisplayAlertText("No admin with that name found.");
                }
            }
            FileHandler.SaveUsersToJson(users);
            break;
        case 3:
            {
                Console.WriteLine("A list of all admins");

                foreach (User user in users.Where(user => user.GetRole() == Role.Admin))
                // foreach(User user in users)
                {
                    // if(user.GetRole() == Role.Admin && user.checkpermissions() == Permissions.None)
                    Console.WriteLine($"{user.ToString()}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string adminName = Utils.GetRequiredInput("Pick admin name you want to handle:  ");
                IUser? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (adminUser != null)
                {
                    string acceptOrDeny = Utils.GetRequiredInput($"You chose: {adminUser.Username}, Do you want accept(y) or deny(d) the permission for handling registration requests?");
                    switch (acceptOrDeny)
                    {
                        case "y":
                            adminUser.GrantPermission(Permissions.AddPersonell); // <-- anropa metoden, lägg till permission till listan över permission för den admin
                            Utils.DisplaySuccesText($"You have accepted the permission to create personel for admin: {adminName} ");
                            break;

                        case "d":
                            adminUser.GrantPermission(Permissions.AddPersonell);   // <-- anropa metoden, ta bort permissions i listan över permissions. Om listan är tom sätt permissions till None. 
                            Utils.DisplaySuccesText($"You have denied permission create personel for user: {adminName} ");
                            break;
                        default:
                            Utils.DisplayAlertText("Only y or n is handled");
                            break;
                    }
                }
                else
                {
                    Utils.DisplayAlertText(".");
                }
            }
            FileHandler.SaveUsersToJson(users);
            break;
        case 4:
            {
                Console.WriteLine("A list of all admins");

                foreach (User user in users.Where(user => user.GetRole() == Role.Admin))
                // foreach(User user in users)
                {
                    // if(user.GetRole() == Role.Admin && user.checkpermissions() == Permissions.None)
                    Console.WriteLine($"{user.ToString()}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string adminName = Utils.GetRequiredInput("Pick admin name you want to handle:  ");
                IUser? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (adminUser != null)
                {
                    string acceptOrDeny = Utils.GetRequiredInput($"You chose: {adminUser.Username}, Do you want accept(y) or deny(d) the permission for viewing all users permissions?");
                    switch (acceptOrDeny)
                    {
                        case "y":
                            adminUser.GrantPermission(Permissions.AddAdmin); // <-- anropa metoden, lägg till permission till listan över permission för den admin
                            Utils.DisplaySuccesText($"You have accepted the permission to view all user permissions for admin: {adminName} ");
                            break;

                        case "d":
                            adminUser.GrantPermission(Permissions.AddAdmin);   // <-- anropa metoden, ta bort permissions i listan över permissions. Om listan är tom sätt permissions till None. 
                            Utils.DisplaySuccesText($"You have denied permission to view all user permissions for user: {adminName} ");
                            break;
                        default:
                            Utils.DisplayAlertText("Only y or n is handled");
                            break;
                    }
                }
                else
                {
                    Utils.DisplayAlertText("No admin with that name found.");
                }
            }
            FileHandler.SaveUsersToJson(users);
            break;
        case 5:
            {

                if (activeUser.GetRole() == Role.SuperAdmin)
                {
                    Console.WriteLine("\nAll admins with pending request:");
                    foreach (User user in users.Where(user => user.GetRole() == Role.Admin && user.GetRegistration() == Registration.Pending))
                    {
                        Console.WriteLine($"{user.ToString()}");
                    }
                    // Work with string get name first and after we are done we are working with index. 
                    string adminHandling = Utils.GetRequiredInput("Pick admin username you want to handle:  ");
                    IUser? adminUser = users.Find(user => user.Username.Equals(adminHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                    if (adminUser != null)
                    {
                        string acceptOrDeny = Utils.GetRequiredInput($"You picked: {adminUser.Username}, Do you want accept(y) or deny(d) the request:  ");
                        switch (acceptOrDeny)
                        {
                            case "y":
                                adminUser.AcceptPending(); // <-- anropa metoden
                                Utils.DisplaySuccesText("Admin registration accepted");
                                break;

                            case "d":
                                adminUser.DenyPending();   // <-- anropa metoden
                                Utils.DisplaySuccesText("Admin registration denied");
                                break;
                            default:
                                Utils.DisplayAlertText("Only y or n is handled");
                                break;
                        }
                    }
                    else
                    {
                        Utils.DisplayAlertText("No admin by that name has been found");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
                break;
            }

        case 6:
            ShowSchedule(activeUser);
            break;

        case 7:
            Console.WriteLine("Logging out...");
            FileHandler.SaveUsersToJson(users);
            activeUser = null;
            break;
        default:
            Utils.DisplayAlertText("Invalid input. Please try again.");
            break;
    }
}



// ============================
// ADMIN MENU METHOD
// ============================
static void AdminMenu(List<IUser> users, List<Location> locations, IUser activeUser)
{
    Console.WriteLine("\n(Admin) Options:");
    Console.WriteLine("1. Create account");
    Console.WriteLine("2. See list of all users");
    Console.WriteLine("3. Add location");
    Console.WriteLine("4. View all locations");
    Console.WriteLine("5. See pending patient request");
    Console.WriteLine($"6. See user permissions");
    Console.WriteLine("7. View my schedule");
    Console.WriteLine("8. Logout");

    switch (Utils.GetIntegerInput("Choice:"))
    {
        case 1:
            Console.WriteLine("Create account for personel and admin");
            if (activeUser.HasPermission(Permissions.AddPersonell))
            {
                Console.WriteLine("(1). Create account for Personell");
            }
            if (activeUser.HasPermission(Permissions.AddAdmin))
            {
                Console.WriteLine("(2). Create account for Admin");
            }
            Console.WriteLine("(3). Go up");
            // Console.WriteLine("(3) Go up");
            switch (Utils.GetIntegerInput("Choose a number: "))
            {
                case 1:
                    if (!activeUser.HasPermission(Permissions.AddPersonell))
                    {
                        Utils.DisplayAlertText("You cant do that.");
                        break;
                    }
                    else
                    {
                        string newUser = Utils.GetRequiredInput("Insert username: ");
                        Console.Write("Insert password: ");
                        string newPass = Utils.GetRequiredInput("Insert password: ");
                        int roleInput = Utils.GetIntegerInput("Pick role: (1)Patient, (2)Personnel, (3)Admin. Choose a number: ");
                        Role role = Role.Patient;
                        if (roleInput == 2) role = Role.Personnel;
                        else if (roleInput == 3) role = Role.Admin;

                        users.Add(new User(Utils.GetIndexAddOne(users), newUser, newPass, role));
                        FileHandler.SaveUsersToJson(users);
                        Utils.DisplaySuccesText("New user created. ");
                    }
                    break;
                case 2:
                    if (!activeUser.HasPermission(Permissions.AddAdmin))
                    {
                        Utils.DisplayAlertText("You cant do that.");
                        break;
                    }
                    else
                    {

                    }
                    break;
                case 3:
                    break;

            }

            break;
        case 2:
            Console.WriteLine("\nAll users:");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Username} - {user.GetRole()}");
            }
            break;
        case 3:

            if (activeUser.GetRole() == Role.Admin && activeUser.HasPermission(Permissions.AddLocation))
            {
                Console.WriteLine("Please enter the region of the location you wish to add: ");
                string region = Console.ReadLine() ?? "".Trim();

                Console.WriteLine("Please enter the name of the hospital you wish to add: ");
                string hospital = Console.ReadLine() ?? "".Trim();

                locations.Add(new Location(region, hospital));
            }
            else
            {
                System.Console.WriteLine("Access denied. Contact superadmin for permission");
            }


            break;

        case 4:
            Console.WriteLine("All locations currently in the system: \n");
            foreach (var location in locations)
            {
                Console.WriteLine(location.ToString());
            }
            break;
        case 5:
            // bool found = activeUser
            // if(activeUser.GetPermission("addLoc"))
            if (activeUser.GetRole() == Role.Admin && activeUser.HasPermission(Permissions.AddRegistrations))
            {


                Console.WriteLine("\nAll patients with pending request:");
                foreach (User user in users.Where(user => user.GetRole() == Role.Patient && user.GetRegistration() == Registration.Pending))
                {
                    Console.WriteLine($"{user.ToString}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string patientHandling = Utils.GetRequiredInput("Pick patient name you want to handle:  ");
                IUser patientUser = users.Find(user => user.Username.Equals(patientHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (patientUser != null)
                {
                    string acceptOrDeny = Utils.GetRequiredInput($"You choosed: {patientUser.Username}, Do you want accept(y) or deny(d) the request:  ");
                    switch (acceptOrDeny)
                    {
                        case "y":
                            patientUser.AcceptPending(); // <-- anropa metoden
                            Utils.DisplaySuccesText("Correct with accept");
                            break;

                        case "d":
                            patientUser.DenyPending();   // <-- anropa metoden
                            Utils.DisplaySuccesText("Correct with deny");
                            break;
                        default:
                            Utils.DisplayAlertText("Only y or n is handled");
                            break;
                    }
                }
                else
                {
                    Utils.DisplayAlertText("No patient by that name has been found");
                }
            }
            else
            {
                System.Console.WriteLine("Access denied. Contact superadmin for permission");
            }
            break;
        case 6:
            if (activeUser.GetRole() == Role.Admin && activeUser.HasPermission(Permissions.ViewPermissions))
            {
                Console.WriteLine($"\nAll users:");
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Username} - {user.GetRole()} - Permissions: {string.Join(", ", user.PermissionList)}");
                }
            }
            else
            {
                System.Console.WriteLine("Access denied. Contact superadmin for permission");
            }

            break;

        case 7:
            ShowSchedule(activeUser);
            break;

        case 8:
            Console.WriteLine("Logging out...");
            activeUser = null;
            break;

        default:
            Console.WriteLine("Invalid input. Please try again.");
            break;
    }

}

<<<<<<< HEAD
static void PersonnelMenu(List<IUser> users, IUser activeUser, List<Appointment> appointments)
=======

// ============================
// PERSONNEL MENU METHOD
// ============================
static void PersonnelMenu(List<IUser> users, IUser activeUser)
>>>>>>> master
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.Clear();
        Console.WriteLine($"\n(Personnel) Menu - Logged in as {activeUser.Username}");
        Console.WriteLine("1. Open assigned patient journal");
        Console.WriteLine("2. Modify patient appointment"); //Add after Open Journal
<<<<<<< HEAD
        Console.WriteLine("3. View patient journals");
=======
        Console.WriteLine("3. Approve/Deny patient appointment request");
        Console.WriteLine("4. View my schedule");
>>>>>>> master
        Console.WriteLine("4. Logout");

        int input = Utils.GetIntegerInput("\nChoice: ");

        switch (input)
        {
            case 1:
                // Calls the PersonnelUI-function
                PersonnelUI.OpenJournal(users, activeUser);
                break;

            case 2:
                PersonnelUI.ModifyAppointment(users, activeUser);
                break;

<<<<<<< HEAD
            case 3:
                /* if (activeUser.GetRole() == Role.Personnel && activeUser.HasPermission("ViewPatientJournal"))
                { */
                foreach (User user in users)
                {
                    if (user.GetRole() == Role.Patient)
                    {
                        Console.WriteLine(user.Username);
                    }
                }

                // Work with string get name first and after we are done we are working with index. 
                string patientHandling = Utils.GetRequiredInput("Pick patient name you want to handle:  ");
                IUser? patientUser = users.Find(user => user.Username.Equals(patientHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                if (patientUser != null)
                {

                    Console.WriteLine();
                    Console.ReadLine();
                }
                /*    else
                   {
                       Utils.DisplayAlertText("No patient by that name has been found");
                   } */
                /* } */
                else
                {
                    Console.WriteLine("Access denied. Contact superadmin for permission");
                }
                break;
            case 4:
=======
            case 3: //Aprove/Deny patient appointment request
                PersonnelUI.ApproveAppointments(users, activeUser);
                break;


            case 4:
                ShowSchedule(activeUser);
                break;

            case 5:
>>>>>>> master
                Console.WriteLine("Logging out...");
                inMenu = false;
                break;

            default:
                Utils.DisplayAlertText("Invalid option. Please try again.");
                break;

        }
    }

}

// ============================
// PATIENT MENU METHOD
// ============================
static void PatientMenu(IUser activeUser)
{
    // Initialize ScheduleService (handles JSON read/write)
    ScheduleService scheduleService = new ScheduleService();

    bool inMenu = true;

    while (inMenu)
    {
        Console.Clear();
        Console.WriteLine("\n(Patient) Menu Choices:");
        Console.WriteLine("1. See Journal");
        Console.WriteLine("2. Book appointment");
        Console.WriteLine("3. See my appointments");
        Console.WriteLine("4. Cancel appointment");
        Console.WriteLine("5. View my doctors (mock)");
        Console.WriteLine("6. View my schedule");
        Console.WriteLine("7. Logout");

        int input = Utils.GetIntegerInput("\nChoice: ");

        switch (input)
        {
            // ==========================================
            // CASE 1 — View journal (placeholder)
            // ==========================================
            case 1:
                Console.Clear();
                Console.WriteLine($"--- Patient Journal for {activeUser.Username} ---\n");

                //Create JournalService instance 
                var journalService = new JournalService();

                //Load journal entries for this patient
                var entries = journalService.GetJournalEntries(activeUser.Id);

                //Display entries
                if (entries.Count == 0)
                {
                    Console.WriteLine("(No journal entries found)");
                }
                else
                {
                    foreach (var entry in entries)
                    {
                        Console.WriteLine(entry.Format());
                    }
                }
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
                break;


            // ==========================================
            // CASE 2 — Book a new appointment
            // ==========================================
            case 2:
                Console.WriteLine("\n--- Create New Appointment ---");

                string doctor = Utils.GetRequiredInput("Doctor's name: ");
                string department = Utils.GetRequiredInput("Department / Location: ");
                string type = Utils.GetRequiredInput("Type of appointment (e.g., checkup, consultation): ");
                string dateInput = Utils.GetRequiredInput("Date and time (format: yyyy-MM-dd HH:mm): ");

                // Validate date input
                if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", null,
                    System.Globalization.DateTimeStyles.None, out DateTime appointmentDate))
                {
                    Utils.DisplayAlertText("Invalid date format. Please use yyyy-MM-dd HH:mm");
                    Console.ReadKey();
                    break;
                }

                // Create and save new appointment
                Appointment newAppointment = new Appointment(activeUser.Id, appointmentDate, doctor, department, type);
                scheduleService.SaveAppointment(newAppointment);

                Utils.DisplaySuccesText($"Appointment with {doctor} on {appointmentDate:yyyy-MM-dd HH:mm} has been booked.");
                Console.ReadKey();
                break;

            // ==========================================
            // CASE 3 — View all appointments
            // ==========================================
            case 3:
                Console.WriteLine("\n--- Your Appointments ---");

                // Load schedule from JSON
                Schedule mySchedule = scheduleService.LoadSchedule(activeUser.Id);

                if (mySchedule.Appointments.Count == 0)
                {
                    Utils.DisplayAlertText("You have no upcoming appointments.");
                }
                else
                {
                    mySchedule.PrintSchedule();
                }

                Console.WriteLine("\nPress ENTER to return to menu...");
                Console.ReadLine();
                break;

            // ==========================================
            // CASE 4 — Cancel an existing appointment
            // ==========================================
            case 4:
                Console.WriteLine("\n--- Cancel Appointment ---");

                Schedule cancelSchedule = scheduleService.LoadSchedule(activeUser.Id);
                if (cancelSchedule.Appointments.Count == 0)
                {
                    Utils.DisplayAlertText("You have no appointments to cancel.");
                    Console.ReadKey();
                    break;
                }

                cancelSchedule.PrintSchedule();

                string cancelInput = Utils.GetRequiredInput("\nEnter the exact date and time of the appointment to cancel (yyyy-MM-dd HH:mm): ");

                if (!DateTime.TryParseExact(cancelInput, "yyyy-MM-dd HH:mm", null,
                    System.Globalization.DateTimeStyles.None, out DateTime cancelDate))
                {
                    Utils.DisplayAlertText("Invalid date format.");
                    Console.ReadKey();
                    break;
                }

                // Attempt to remove the appointment from JSON
                scheduleService.RemoveAppointment(activeUser.Id, cancelDate);
                Utils.DisplaySuccesText("Appointment canceled (if it existed).");
                Console.ReadKey();
                break;

            // ==========================================
            // CASE 5 — Mock doctors list
            // ==========================================
            case 5:
                Console.WriteLine("\n--- Your Doctors ---");
                Console.WriteLine("Dr. Smith - Cardiology");
                Console.WriteLine("Dr. Lewis - Orthopedics");
                Console.WriteLine("Dr. Andersson - General Medicine");
                Console.WriteLine("\nPress ENTER to return...");
                Console.ReadLine();
                break;

            //CASE 6
            case 6:
                ShowSchedule(activeUser);
                break;

            // ==========================================
            // CASE 7 — Logout
            // ==========================================
            case 7:
                Console.WriteLine("Logging out...");
                inMenu = false;
                break;

            default:
                Utils.DisplayAlertText("Invalid option, please try again.");
                break;
        }

    }

}
