using App;
/* 

-- SAVE AND MOCK DATA TO TEXT FILES --

As a user, I need to be able to log in. CHECKED
As a user, I need to be able to log out.
As a user, I need to be able to request registration as a patient.


As an admin with sufficient permissions, I need to be able to give admins the permission to handle the permission system, in fine granularity.
As an admin with sufficient permissions, I need to be able to assign admins to certain regions.
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations.
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations.
As an admin with sufficient permissions, I need to be able to give admins the permission to add locations.
As an admin with sufficient permissions, I need to be able to give admins the permission to create accounts for personnel.
As an admin with sufficient permissions, I need to be able to give admins the permission to view a list of who has permission to what.
As an admin with sufficient permissions, I need to be able to add locations.
As an admin with sufficient permissions, I need to be able to accept user registration as patients.
As an admin with sufficient permissions, I need to be able to deny user registration as patients.
As an admin with sufficient permissions, I need to be able to create accounts for personnel.
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
class Program
{
    static void Main()
    {
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
                string username = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();

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
                        AdminMenu(users);
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
    static void AdminMenu(List<IUser> users)
    {
        Console.WriteLine("\n(Admin) Alternativ:");
        Console.WriteLine("1. Create account");
        Console.WriteLine("2. See list of all users");

        Console.Write("Choice: ");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.Write("Insert username: ");
            string newUser = Console.ReadLine();
            Console.Write("Insert password: ");
            string newPass = Console.ReadLine();

            Console.WriteLine("Pick role: 1=Patient, 2=Personnel, 3=Admin");
            string roleInput = Console.ReadLine();
            Role role = Role.Patient;
            if (roleInput == "2") role = Role.Personnel;
            else if (roleInput == "3") role = Role.Admin;

            users.Add(new User(newUser, newPass, role));
            Console.WriteLine("New user created. ");
        }
        else if (choice == "2")
        {
            Console.WriteLine("\nAll users:");
            foreach (var u in users)
            {
                Console.WriteLine($"{u.Username} - {u.GetRole()}");
            }
        }
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
            Console.WriteLine("Your journal: mock");
        }
        else if (input == "2")
        {
            Console.WriteLine("Appointment created (mock)");
        }
    }
}
