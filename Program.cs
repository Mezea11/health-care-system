using App;
/* 

-- SAVE AND MOCK DATA TO TEXT FILES --

As a user, I need to be able to log in. CHECKED
As a user, I need to be able to log out. CHECKED
As a user, I need to be able to REQUEST registration as a patient.


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

locations.Add(new Location("Skåne", "Lunds Universitetssjukhus"));
locations.Add(new Location("Stockholm", "Karolinska institutet"));

// Lista med alla användare 
List<IUser> users = new List<IUser>()
            {
                new User("patient", "123", Role.Patient),
                new User("personell", "123", Role.Personnel),
                new User("admin", "123", Role.Admin)
            };

IUser? activeUser = null;
bool running = true;

// Main program loop
while (running)
{
    Console.Clear();

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
        if (activeUser.GetRegistration() == Registration.Pending && activeUser.GetRole() == Role.Patient)
        {
            Utils.DisplayAlertText("Your account is still pending. Need to wait for the admin to accept it, Press ENTER to continue");
            Console.ReadKey();
            activeUser = null;

        }
        else
        {

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
                    AdminMenu(users, locations);
                    break;

                // PERSONNEL MENU
                case Role.Personnel:
                    PersonnelMenu();
                    break;

                // PATIENT MENU
                case Role.Patient:
                    PatientMenu();
                    break;

            }

            Console.WriteLine("\nWrite 'logout' or press Enter to continue.");
            string input = Console.ReadLine();
            if (input == "logout") activeUser = null;
        }
    }
}


// ============================
// ADMIN MENU METHOD
// ============================
static void AdminMenu(List<IUser> users, List<Location> locations)
{
    Console.WriteLine("\n(Admin) Options:");
    Console.WriteLine("1. Create account");
    Console.WriteLine("2. See list of all users");
    Console.WriteLine("3. Add location");
    Console.WriteLine("4. View all locations");
    Console.WriteLine("5. See pending patient request");

    // Console.Write("Choice: ");
    // string choice = Console.ReadLine();
    switch (Utils.GetIntegerInput("Choice:"))
    {
        case 1:
            string newUser = Utils.GetRequiredInput("Insert username: ");
            Console.Write("Insert password: ");
            string newPass = Utils.GetRequiredInput("Insert password: ");
            string roleInput = Utils.GetRequiredInput("Pick role: 1=Patient, 2=Personnel, 3=Admin");
            Role role = Role.Patient;
            if (roleInput == "2") role = Role.Personnel;
            else if (roleInput == "3") role = Role.Admin;

            users.Add(new User(newUser, newPass, role));
            Console.WriteLine("New user created. ");
            break;
        case 2:
            Console.WriteLine("\nAll users:");
            // Change the var to User and no oneletter variable
            foreach (var u in users)
            {
                Console.WriteLine($"{u.Username} - {u.GetRole()} - {u.GetRegistration()}");
            }
            break;
        case 3:
            Console.WriteLine("Please enter the region of the location you wish to add: ");
            string region = Console.ReadLine() ?? "".Trim();

            Console.WriteLine("Please enter the name of the hospital you wish to add: ");
            string hospital = Console.ReadLine() ?? "".Trim();

            locations.Add(new Location(region, hospital));

            break;

        case 4:
            Console.WriteLine("All locations currently in the system: \n");
            foreach (var location in locations)
            {
                Console.WriteLine(location.ToString());
            }
            break;
        case 5:
            Console.WriteLine("\nAll patients with pending request::");
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


// ============================
// PERSONNEL MENU METHOD
// ============================
static void PersonnelMenu()
{
    Console.WriteLine("\n(Personnel) Menu Choices:");
    Console.WriteLine("1. See schedule");
    Console.WriteLine("2. Accept booking (mock)");
    Console.Write("Val: ");
    string input = Console.ReadLine();

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
static void PatientMenu()
{
    Console.WriteLine("\n(Patient) Menu Choices:");
    Console.WriteLine("1. See Journal (mock)");
    Console.WriteLine("2. Book appointment (mock)");
    Console.Write("Choice: ");
    string input = Console.ReadLine();

    if (input == "1")
    {
        Console.WriteLine("Your journal: mock journal");
    }
    else if (input == "2")
    {
        Console.WriteLine("Appointment created (mock)");
    }
}
