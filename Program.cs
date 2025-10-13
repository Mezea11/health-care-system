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
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations.
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations.
As an admin with sufficient permissions, I need to be able to give admins the permission to add locations.
As an admin with sufficient permissions, I need to be able to give admins the permission to create accounts for personnel.
As an admin with sufficient permissions, I need to be able to give admins the permission to view a list of who has permission to what.
As an admin with sufficient permissions, I need to be able to add locations.

As an admin with sufficient permissions, I need to be able to accept user registration as patients. -- CHECKED
As an admin with sufficient permissions, I need to be able to deny user registration as patients. -- CHECKED
As an admin with sufficient permissions, I need to be able to create accounts for personnel. CHECKED

As an admin with sufficient permissions, I need to be able to view a list of who has permission to what.


As personnel with sufficient permissions, I need to be able to view a patients journal entries.
As personnel with sufficient permissions, I need to be able to mark journal entries with different levels of read permissions.
As personnel with sufficient permissions, I need to be able to register appointments.
As personnel with sufficient permissions, I need to be able to modify appointments.
As personnel with sufficient permissions, I need to be able to approve appointment requests.
As personnel with sufficient permissions, I need to be able to view the schedule of a location.


As a patient, I need to be able to view my own journal.
As a patient, I need to be able to request an appointment.
As a logged in user, I need to be able to view my schedule.
*/




// ============================
// Main program
// ============================
List<Location> locations = new List<Location>();
List<Appointment> appointments = new List<Appointment>();
locations.Add(new Location("Skåne", "Lunds Universitetssjukhus"));
locations.Add(new Location("Stockholm", "Karolinska institutet"));

// Lista med alla användare 
List<IUser> users = new List<IUser>();

users.Add(new User(0, "patient", "123", Role.Patient));
users.Add(new User(1, "personell", "123", Role.Personnel));
users.Add(new User(2, "admin", "123", Role.Admin));
users.Add(new User(3, "superadmin", "123", Role.SuperAdmin));
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
        Console.WriteLine("1. For sending request for registration as patient");
        Console.WriteLine("2. Log In");
        // Console.Write("Choice: ");
        // string choice = Console.ReadLine();

        switch (Utils.GetIntegerInput("Choice a number: "))
        {
            case 1:
                // CREATE LOGIC IN HERE TO REGISTER A NEW USER
                Console.WriteLine("Type in your username");// PROMPT USER TO INSERT USERNAME
                string newUser = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Type in your password"); // PROMPT USER TO INSERT PASSWORD
                string newPass = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Request Sent.");
                users.Add(new User(GetIndexAddOne(users), newUser, newPass, Role.Patient));  // CREATE NEW OBJECT (WITH ROLE PATIENT) WITH USERNAME AND PASSWORD
                break;

            case 2:
                MainMenu();
                break;
        }
    }

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
            Console.Write("Username: ");
            string username = Utils.GetRequiredInput("Username: ");
            Console.Write("Password: ");
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
                Console.WriteLine("Wrong login credentials. Press enter to try again");
                Console.ReadLine();
            }
        }

        else
        {
            // If user as a role as patient has a registratin not yet handled by admin. Will not be able to login. But it will have a active_user as not null
            if (activeUser.GetRegistration() == Registration.Pending && activeUser.GetRole() == Role.Patient)
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

            switch (activeUser.GetRole())
            {

                // ADMIN MENU
                case Role.Admin:
                    AdminMenu(users, locations, activeUser);
                    break;

                // PERSONNEL MENU
                case Role.Personnel:
                    PersonnelMenu();
                    break;

                // PATIENT MENU
                case Role.Patient:
                    PatientMenu(activeUser, appointments);
                    break;

                // SUPERADMIN MENU
                case Role.SuperAdmin:
                    SuperAdminMenu(users, locations);
                    break;

            }

            Console.WriteLine("\nWrite 'logout' or press Enter to continue.");
            string input = Console.ReadLine();
            if (input == "logout") activeUser = null;
        }
    }
}



// ============================
// SUPERADMIN MENU METHOD
// ============================
static void SuperAdminMenu(List<IUser> users, List<Location> locations)
{
    Console.WriteLine("\n(SuperAdmin) Options:");
    Console.WriteLine("1. Grant admin to add location");

    string input = Console.ReadLine() ?? "".Trim();

    switch (input)
    {
        case "1":
            Console.WriteLine("A list of all admins");

            foreach (User user in users.Where(user => user.GetRole() == Role.Admin))
            // foreach(User user in users)
            {
                // if(user.GetRole() == Role.Admin && user.checkpermissions() == Permissions.None)
                Console.WriteLine($"{user.ToString()} | {user.GetPermissions()}");
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
                        adminUser.AcceptAddLocationPermission(); // <-- anropa metoden
                        Utils.DisplaySuccesText("You have accepted the permission");
                        break;

                    case "d":
                        adminUser.DenyAddLocationPermission();   // <-- anropa metoden
                        Utils.DisplaySuccesText("You have denied the permission");
                        break;
                    default:
                        Utils.DisplayAlertText("Only y or n is handled");
                        break;
                }
            }
            else
            {
                Utils.DisplayAlertText("Ingen patient med det namnet hittades.");
            }
            break;

    }
}



// ============================
// ADMIN MENU METHOD
// ============================
static void AdminMenu(List<IUser> users, List<Location> locations, IUser? activeUser)
{
    Console.WriteLine("\n(Admin) Options:");
    Console.WriteLine("1. Create account");
    Console.WriteLine("2. See list of all users");
    Console.WriteLine("3. Add location");
    Console.WriteLine("4. View all locations");
    Console.WriteLine("5. See pending patient request");
    Console.WriteLine("6. Logout");

    // Console.Write("Choice: ");
    // string choice = Console.ReadLine();
    switch (Utils.GetIntegerInput("Choice:"))
    {
        case 1:
            string newUser = Utils.GetRequiredInput("Insert username: ");
            Console.Write("Insert password: ");
            string newPass = Utils.GetRequiredInput("Insert password: ");
            int roleInput = Utils.GetIntegerInput("Pick role: (1)Patient, (2)Personnel, (3)Admin. Choose a number: ");
            Role role = Role.Patient;
            if (roleInput == 2) role = Role.Personnel;
            else if (roleInput == 3) role = Role.Admin;

            users.Add(new User(GetIndexAddOne(users), newUser, newPass, role));
            Utils.DisplaySuccesText("New user created. ");
            break;
        case 2:
            Console.WriteLine("\nAll users:");
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Username} - {user.GetRole()}");
            }
            break;
        case 3:
            if (activeUser.GetRole() == Role.Admin && activeUser.GetPermissions() == Permissions.AddLocation)
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
            Console.WriteLine("\nAll patients with pending request:");
            foreach (User user in users.Where(user => user.GetRole() == Role.Patient && user.GetRegistration() == Registration.Pending))
            {
                Console.WriteLine($"{user.Username} - {user.GetRole()}");
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
                Utils.DisplayAlertText("Ingen patient med det namnet hittades.");
            }
            break;

        case 6:
            Console.WriteLine("Logging out...");
            activeUser = null;
            break;

        default:
            Console.WriteLine("Invalid input. Please try again.");
            break;
    }
    //     if (choice == "1")
    //     {
    //  
    //     }
    //     else if (choice == "2")
    //     {
    //         Console.WriteLine("\nAll users:");
    //         foreach (var u in users)
    //         {
    //             Console.WriteLine($"{u.Username} - {u.GetRole()}");
    //         }
    //     }
}

static int GetIndexAddOne(List<IUser> users)
{
    return users.Count() + 1;
}
// ============================
// PERSONNEL MENU METHOD
// ============================
static void PersonnelMenu()
{
    Console.WriteLine("\n(Personnel) Menu Choices:");
    Console.WriteLine("1. See schedule");
    Console.WriteLine("2. Accept booking (mock)");
    string input = Utils.GetRequiredInput("Choice: ");

    if (input == "1")
    {
        Console.WriteLine("Schema visas (mock)...");
    }
    else if (input == "2")
    {
        Console.WriteLine("Bokningar godkända (mock)...");
    }
}

// ============================
// PATIENT MENU METHOD
// ============================
static void PatientMenu(IUser activeUser, List<Appointment> appointments)
{
    //Create ScheduleService and load the patient's schedule.
    ScheduleService scheduleService = new ScheduleService();
    Schedule schedule = scheduleService.LoadSchedule(activeUser.Id);

    //Create JournalService (for reading journal entries).
    JournalService journalService = new JournalService();

    bool running = true;

    while (running)
    {
        Console.Clear();
        Console.WriteLine($"\n(Patient) Logged in as {activeUser.Username}");
        Console.WriteLine("1. See Journal");
        Console.WriteLine("2. Book appointment");
        Console.WriteLine("3. See my appointment");
        Console.WriteLine("4. Cancel appointments");
        Console.WriteLine("5. View my doctors");
        Console.WriteLine("6. Logout");

        int input = Utils.GetIntegerInput("Choice: ");

        switch (input)
        {
            case 1:
                Console.WriteLine("\n--- Your Journal ---");
                var entries = journalService.LoadJournal(activeUser.Id);
                if (!entries.Any())
                    Console.WriteLine("No journal entrie found.");
                else
                    foreach (var entry in entries)
                        Console.WriteLine(entry);
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
                break;

            case 2:
                Console.WriteLine("\n--- Book a new appointment ---");
                string type = Utils.GetRequiredInput("What kind of appointment?: ");
                string doctor = Utils.GetRequiredInput("Which doctor do you want to see?: ");
                string location = Utils.GetRequiredInput("Which location do you want to see?: ");
                string dateInput = Utils.GetRequiredInput("Enter date (yyyy-MM-dd HH:mm): ");
                DateTime date = DateTime.ParseExact(dateInput, "yyyy-MM-dd HH:mm", null);

                Appointment newApp = new Appointment(activeUser.Id, date, doctor, location, type);

                //Add to schedule and save to file.
                schedule.AddAppointment(newApp);
                scheduleService.SaveAppointment(newApp);

                Console.WriteLine("Appointment successfully booked! Press enter to continue.");
                Console.ReadLine();
                break;

            case 3:
                Console.WriteLine("\n--- Your Appointments ---");
                if (!schedule.Appointments.Any())
                    Console.WriteLine("No appointments found.");
                else
                    schedule.PrintSchedule();
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
                break;
        }

    }
    Console.WriteLine("\n(Patient) Menu Choices:");
    Console.WriteLine("1. See Journal");
    Console.WriteLine("2. Book appointment");
    Console.WriteLine("3. See my appointments");
    Console.WriteLine("4. Cancel appointments");
    Console.WriteLine("5. View my doctors");
    Console.WriteLine("6. Logout");
    // Console.Write("Choice: ");
    int input = Utils.GetIntegerInput(" Choice: ");

    if (input == 1)
    {
        Console.WriteLine("Your journal: mock journal");
    }
    else if (input == 2)
    {
        Console.WriteLine("\n--- Book a new appointment ---");
        string type = Utils.GetRequiredInput("Waht kind of appointment?: ");
        string doctor = Utils.GetRequiredInput("Which doctor do you want to see?: ");
        string location = Utils.GetRequiredInput("Which location do you want to see?: ");
        string dateInput = DateTime.ParseExact(dateInput, "yyyy-MM-dd HH:mm" null);

        Appointment newApp = new Appointment(activeUser.Id, date, doctor, location, type);

        Schedule.AddAppointment(newApp);
        scheduleservice.SaveAppointment(newApp);
        Console.WriteLine("Appointment successfully booked!");



    }
    else if (input == 3)
    {
        Console.WriteLine("See my appointments");
        foreach (Appointment app in appointments.Where(appointment => appointment.UserId == activeUser.Id))
        {
            Console.WriteLine($"{app.Format()}");
        }
        //return $"{Date:yyyy-MM-dd HH:mm} | {Doctor,-15} | {Department,-15} | {Type}";
    }
    else if (input == 4)
    {
        Console.WriteLine("Cancel appointments");
    }
    else if (input == 5)
    {
        Console.WriteLine("View my doctors");
    }
    else if (input == 6)
    {
        Console.WriteLine("Logout...");
    }
}
