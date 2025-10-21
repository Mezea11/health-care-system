using App;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
/* 

-- SAVE AND MOCK DATA TO TEXT FILES --

As a user, I need to be able to log in. CHECKED
As a user, I need to be able to log out. CHECKED
As a user, I need to be able to REQUEST registration as a patient. CHECKED 


As an admin with sufficient permissions, I need to be able to give admins the permission to handle the permission system, in fine granularity.
As an admin with sufficient permissions, I need to be able to assign admins to certain regions.  ---- Does it works ?? 
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations. CHECKED
As an admin with sufficient permissions, I need to be able to give admins the permission to handle registrations. CHECKED
As an admin with sufficient permissions, I need to be able to give admins the permission to add locations. CHECKED
As an admin with sufficient permissions, I need to be able to give admins the permission to create accounts for personnel. CHECKED
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



static int GetIndexAddOne(List<User> users)
{
    return users.Count + 1;
}
// ============================
// Main program
// ============================

List<Location> locations = new List<Location>();
List<Appointment> appointments = new List<Appointment>();
/* List<AdminLocation> adminLocations = new List<AdminLocation>();
 */
locations.Add(new Location("Skåne", "Lunds Universitetssjukhus"));
locations.Add(new Location("Stockholm", "Karolinska institutet"));

// Lista med alla användare 
// List<User> users = FileHandler.LoadUsersFromJson();
List<User> users = FileHandler.LoadFromCsv();

User? activeUser = null;
bool running = true;


StartMenu(users);

// ============================
// START MENU METHOD
// ============================

// THIS METHOD ALLOWS USER TO EITHER LOGIN OR REGISTER
void StartMenu(List<User> users)
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

                // Console.WriteLine("Type in your password"); // PROMPT USER TO INSERT PASSWORD
                string newPass = Utils.GetRequiredInput("Tpe in your password: ");
                Console.Clear();

                Console.WriteLine("Request Sent.");
                users.Add(new User(Utils.GetIndexAddOne(users), newUser, newPass, Role.Patient));  // CREATE NEW OBJECT (WITH ROLE PATIENT) WITH USERNAME AND PASSWORD
                FileHandler.SaveUsersToCsv(users);
                break;
            case 2:
                string newAdmin = Utils.GetRequiredInput("Type in your username: "); // PROMPT USER TO INSERT USERNAME
                Console.Clear();

                Console.WriteLine("Type in your password"); // PROMPT USER TO INSERT PASSWORD
                string newAdminPass = Console.ReadLine() ?? "".Trim();
                Console.Clear();

                Console.WriteLine("Request Sent.");
                users.Add(new User(GetIndexAddOne(users), newAdmin, newAdminPass, Role.Admin));


                FileHandler.SaveUsersToCsv(users);

                break;

            case 3:
                MainMenu();
                break;
        }
    }

}
//COMMON METHOD - Show current user's schedule
static void ShowSchedule(User activeUser)
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
            foreach (User user in users)
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
                    PatientMenu(activeUser,
    users.Where(user =>
        // Filter out users that dont have the role as personel and persoal role as doctor
        user.GetRole() == Role.Personnel &&
        user.PersonelRole == PersonellRoles.Doctor)
    .ToList(), users); // we add the whole users list because we need it when we save to tje json file

                    break;

                // SUPERADMIN MENU
                case Role.SuperAdmin:
                    SuperAdminMenu(users, locations, activeUser);
                    break;

            }
            // Console.WriteLine("\nWrite 'logout' or press Enter to continue.");
            // string? input = Console.ReadLine();
            // if (input == "logout") activeUser = null;
        }
    }
}





/// ============================
// SUPERADMIN MENU METHOD
// ============================
static void SuperAdminMenu(List<User> users, List<Location> locations, User activeUser) // creates a menu for superadmin with list of users and locations
{
    Console.WriteLine("\n(SuperAdmin) Options:");
    Console.WriteLine("1. Grant admin to add location");
    Console.WriteLine("2. Overview of permissions");
    Console.WriteLine("3. Grant admin to handle registrations");
    Console.WriteLine("4. Grant admin to create personel");
    Console.WriteLine("5. Grant admin to check list of user permissions");
    Console.WriteLine("6. See pending admin registration requests");
    Console.WriteLine("7. Assign admins to certain regions");
    Console.WriteLine("8. Logout");


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
                User? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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
            FileHandler.SaveUsersToCsv(users);
            break;

        case 2:
            Console.WriteLine("Overview regarding the permissions for all users");

            foreach (User user in users)
            {
                Console.WriteLine(user.ToString());
            }
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
                User? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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
            FileHandler.SaveUsersToCsv(users);
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
                User? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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

            FileHandler.SaveUsersToCsv(users);
            break;
        case 5:
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
                User? adminUser = users.Find(user => user.Username.Equals(adminName, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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
            FileHandler.SaveUsersToCsv(users);
            break;

        case 6:
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
                    User? adminUser = users.Find(user => user.Username.Equals(adminHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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
        case 7:
            Console.WriteLine("Assign admins to regions");

            if (!users.Any(user => user.GetRole() == Role.Admin)) //loopar genom alla användare
            {
                Utils.DisplayAlertText("No admins found"); //om ingen admin hittas.
                break;
            }
            List<User> AdminList = new List<User>(); //annars skapas en ny lista med bara admins genom index 
            for (int i = 0; i < users.Count; ++i) //för att det ska bli enklare för superadmin att välja genom siffror istället för text
            {
                if (users[i].GetRole() == Role.Admin)
                {
                    AdminList.Add((User)users[i]); //listan spottas ut för varje admin-användare
                    Console.WriteLine(AdminList.Count + ": " + users[i].Username);
                }
            }
            int chosenIndex = Utils.GetIntegerInput("Choose an admin by number: ") - 1; //väljer den admin du vill tilldela en region genom en siffra
            if (chosenIndex < 0 || chosenIndex >= AdminList.Count)
            {
                Utils.DisplayAlertText("Invalid number. No admin assigned."); //om du väljer en siffra som inte finns med på den utspottade listan
                break;
            }

            User chosenAdmin = AdminList[chosenIndex];
            Region[] regions = (Region[])Enum.GetValues(typeof(Region)); // get all values from the region enum to display as selectable options
            for (int i = 0; i < regions.Length; i++)
            {
                Console.WriteLine($"{i + 1}: {regions[i]}");
            }

            int regionChoice = Utils.GetIntegerInput($"Choose a region for {chosenAdmin.Username}: ") - 1;
            if (regionChoice < 0 || regionChoice >= regions.Length)
            {
                Utils.DisplayAlertText("Invalid region choice.");
                break;
            }

            Region selectedRegion = regions[regionChoice];
            chosenAdmin.AssignRegion(selectedRegion);
            Utils.DisplaySuccesText(chosenAdmin.Username + " has been assigned to region: " + selectedRegion);
            break;

        case 8:
            Console.WriteLine("Logging out...");
            FileHandler.SaveUsersToCsv(users);
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
void AdminMenu(List<User> users, List<Location> locations, User activeUser)
{
    Console.WriteLine("\n(Admin) Options:");
    Console.WriteLine("1. Create account");
    Console.WriteLine("2. See list of all users");
    Console.WriteLine("3. Add location");
    Console.WriteLine("4. View all locations");
    Console.WriteLine("5. See pending patient request");
    Console.WriteLine("6. See user permissions");
    Console.WriteLine("7. View my schedule");
    Console.WriteLine("8. View my regions"); // kommer att ändras vid merge.
    Console.WriteLine("9. Logout");

    switch (Utils.GetIntegerInput("Choice:"))
    {
        case 1:
            Console.WriteLine("Create account for personel or admin");
            if (activeUser.HasPermission(Permissions.AddPersonell))
            {
                Console.WriteLine("(1). Create account for Personell");
            }
            if (activeUser.HasPermission(Permissions.AddAdmin))
            {
                Console.WriteLine("(2). Create account for Admin");
            }
            Console.WriteLine("(3). Go up");
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
                        Console.WriteLine("Create new Personel account");
                        string newUser = Utils.GetRequiredInput("Insert username: ");
                        string newPass = Utils.GetRequiredInput("Insert password: ");
                        // create a new user as a role Personell. We need to set a personell role to the object also
                        users.Add(new User(Utils.GetIndexAddOne(users), newUser, newPass, Role.Personnel));
                        User? UserLastCreated = users.Last(); // take the last item in the users list. The element that we create above
                        int chooseRole = Utils.GetIntegerInput("Pick role for the personell: (1)Doctor, (2)Nurse, (3)Administrator. (Choose a number): ");
                        string doctoDetails = "";
                        switch (chooseRole)
                        {
                            case 1:
                                doctoDetails = Utils.GetRequiredInput("Whats the area for the doctor: ");
                                break;
                            case 2:
                            case 3:
                                break;
                        }
                        UserLastCreated.SetRolePersonell(chooseRole, UserLastCreated, doctoDetails);
                        FileHandler.SaveUsersToCsv(users);
                        Utils.DisplaySuccesText($"New personell account for {newUser} created. ");
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
                        Console.WriteLine("Create new Personel account");
                        string newUser = Utils.GetRequiredInput("Insert username: ");
                        string newPass = Utils.GetRequiredInput("Insert password: ");
                        users.Add(new User(Utils.GetIndexAddOne(users), newUser, newPass, Role.Admin));
                        FileHandler.SaveUsersToCsv(users);
                        Utils.DisplaySuccesText($"New admin account for {newUser} created. ");

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
                Console.WriteLine($"Username: {user.Username} - Role: {user.GetRole()}");
                // Console.WriteLine($"{user.Username} - {user.GetRole()} {(user.GetRole() == Role.Personnel && user.PersonelRole == PersonellRoles.Doctor ? $" : { user.PersonelRole} - {user.RoleDetails}" : "")}");
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
                Utils.DisplayAlertText("Access denied. Contact superadmin for permission");
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
                    Console.WriteLine($"{user.ToString()}");
                }
                // Work with string get name first and after we are done we are working with index. 
                string patientHandling = Utils.GetRequiredInput("Pick patient name you want to handle:  ");
                User? patientUser = users.Find(user => user.Username.Equals(patientHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
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
                Utils.DisplayAlertText("Access denied. Contact superadmin for permission");
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
                Utils.DisplayAlertText("Access denied. Contact superadmin for permission");
            }

            break;
        case 7:
            ShowSchedule(activeUser);
            break;

        case 8:
            Console.WriteLine("See my assigned region");
            bool found = false; //boolean created to search for a admin with true or false
            foreach (User user in users)
            {
                if (user.GetRole() == Role.Admin)
                {
                    Region? region = user.GetAssignedRegion();
                    if (region == null || region == Region.None)
                    {
                        Console.WriteLine(user.Username + " has no region assigned.");
                    }
                    else
                    {
                        Console.WriteLine(user.Username + " is assigned to region: " + region);
                    }

                    found = true;
                }
            }
            if (!found)
            {
                Utils.DisplayAlertText("No admins found");
            }
            break;
        case 9:
            Console.WriteLine("Logging out...");
            activeUser = null;
            running = false;
            break;

        default:
            Console.WriteLine("Invalid input. Please try again.");
            break;
    }

}



// ============================
// PERSONNEL MENU METHOD
// ============================

void PersonnelMenu(List<User> users, User activeUser, List<Appointment> appointments)

{
    ScheduleService scheduleService = new ScheduleService();

    bool inMenu = true;
    while (inMenu)
    {
        Console.Clear();
        Console.WriteLine($"\n(Personnel) Menu - Logged in as {activeUser.Username}");
        Console.WriteLine("1. Open assigned patient journal");
        Console.WriteLine("2. Modify patient appointment"); //Add after Open Journal
        Console.WriteLine("3. Approve/Deny patient appointment request");
        Console.WriteLine("4. View my schedule");
        Console.WriteLine("5. View patient journal");
        Console.WriteLine("6. Register appointments");
        Console.WriteLine("7. Logout");


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
            case 3: //Aprove/Deny patient appointment request
                PersonnelUI.ApproveAppointments(users, activeUser);
                break;
            case 4:
                ShowSchedule(activeUser);
                break;
            // VIEW A PATIENT JOURNAL
            case 5:
                /* if (activeUser.GetRole() == Role.Personnel && activeUser.HasPermission("ViewPatientJournal")) */
                {
                    foreach (User user in users)
                    {
                        if (user.GetRole() == Role.Patient)
                        {
                            Console.WriteLine(user.Username);
                        }
                    }
                    // Work with string get name first and after we are done we are working with index. 
                    string patientHandling = Utils.GetRequiredInput("Pick patient name you want to handle:  ");
                    User? patientUser = users.Find(user => user.Username.Equals(patientHandling, StringComparison.OrdinalIgnoreCase)); // refactorerar till en lattlast ://" 
                    if (patientUser != null)
                    {
                        Console.WriteLine(patientUser);
                        Console.ReadLine();
                        Console.WriteLine("Press enter to continue");
                    }


                    else
                    {
                        Utils.DisplayAlertText("No patient by that name has been found");
                    }
                    break;

                }

            case 6:
                {
                    // Loop All users in User
                    foreach (User user in users)

                    {
                        Console.WriteLine(user);
                    }

                    // Input går in och sparas i patientHandling
                    string patientHandling = Utils.GetRequiredInput("Pick patient name you want to handle: ");
                    // Searching through list of users and picks out the one that was saved in patienthandling aaaaand then saving it to patientUser
                    User patientUser = users.Find(user => user.Username.Equals(patientHandling, StringComparison.OrdinalIgnoreCase));
                    // users -> a list<User> your collection of all users
                    //.Find A method that returns the FIRST MATCH based on condition, Returns Null if no match is found
                    // user => "lambda expression" short inline function.
                    // // user.Username.Equals() checks if the current users username equals the input
                    // Patienthandling the input user typed in earlier, the username they want to find
                    // stringComarison.OrdinalIgnoreCase Makes the comparison case-insensetive (so "Alice matches alice)
                    // User patientUser
                    if (patientUser != null)
                    {

                        string department = Utils.GetRequiredInput("Department / location");
                        string type = Utils.GetRequiredInput("Type of appointment (e.g., checkup, consultation)");
                        string dateInput = Utils.GetRequiredInput("Date and time, format (yyyy-MM-dd HH:mm):");

                        if (!DateTime.TryParseExact(dateInput, "yyyy-MM-dd HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime appointmentDate))
                        //Tries to convert a string into a DateTime using exact format
                        //dateInput = users input string
                        //null =? culture info?
                        //DatetimeStyles.none = no special parsing rules applied
                        // out DATETIME APPOINTMENTDATE  if parsing succeed store in appointmentDate
                        {
                            Utils.DisplayAlertText("Invalid date format. Please use yyyy-MM-dd HH:mm");
                            Console.ReadKey();
                            break;
                        }

                        //Add a new appointment in NEWAPP with the () things inside.
                        Appointment newAppointment = new Appointment(patientUser.Id, appointmentDate, "", department, type);

                        //scheduleS an object responsible for handling appointment logic such as sacing, loading or update appointments
                        // saveAppointment() a method that accepts an appointment and stores it
                        // newAppo the actual appointment youre trying to save
                        scheduleService.SaveAppointment(newAppointment);


                        Utils.DisplaySuccesText($"Appointment with {users} on {appointmentDate:yyyy-MM-dd HH:mm} has been booked.");
                        Console.ReadKey();
                        break;


                        //To can choose the user I want
                        // After choosing coming up options to schedule appointment with text and date
                        //Hantera doktorer
                    }
                }
                break;
            case 7:
                Console.WriteLine("Logging out...");
                activeUser = null;
                running = false;
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
void PatientMenu(User activeUser, List<User> doctorsList, List<User> users)
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
        Console.WriteLine("5. Request a doctor");
        Console.WriteLine("6. View my doctors");
        Console.WriteLine("7. View my schedule");
        Console.WriteLine("8. Logout");

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
                Console.WriteLine("All docktors:  ");
                foreach (User user in doctorsList)
                {
                    Console.WriteLine(user.ToPersonnelDisplay());
                }
                string doctor = Utils.GetRequiredInput("Pick a docktor for ypur appointment: ");
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
            // CASE 5 — Request a doctor, from doctors list
            // ==========================================
            case 5:
                Console.WriteLine("\n--- All Doctors to pick from ---");
                foreach (User user in doctorsList)
                {
                    Console.WriteLine(user.ToPersonnelDisplay());
                }
                string doctorName = Utils.GetRequiredInput("Pick the name of the doctor you want to have?? ");
                User? doctorObj = doctorsList.Find(user => user.Username.Equals(doctorName, StringComparison.OrdinalIgnoreCase));
                if (doctorObj != null)
                {
                    bool success = activeUser.AssignPersonnel(doctorObj.Id);
                    if (success)
                    {
                        Utils.DisplaySuccesText($"Personal (ID: {doctorObj.Id}) tilldelad patient {activeUser.Username}.");
                    }
                    else
                    {
                        Utils.DisplayAlertText("Kunde inte tilldela personal. Patienten har redan detta ID, eller det är fel roll.");
                    }
                }
                else
                {
                    Utils.DisplayAlertText("Wront spelling or no doctor by that name");
                }
                Console.WriteLine("\nPress ENTER to return...");
                Console.ReadLine();
                break;
            // ==========================================
            // CASE 6 — All doctors list
            // ==========================================
            case 6:
                Console.WriteLine("\n--- Your Doctors: ---");
                foreach (User user in doctorsList.FindAll(doctor => activeUser.AssignedPersonnelIds.Contains(doctor.Id)))
                {

                    Console.WriteLine(user.ToPersonnelDisplay());
                }
                Console.WriteLine("\nPress ENTER to return...");
                Console.ReadLine();
                break;

            /// ==========================================
            // CASE 7 — Show schedules 
            // ==========================================
            case 7:
                ShowSchedule(activeUser);
                break;

            // ==========================================
            // CASE 7 — Logout
            // ==========================================
            case 8:
                FileHandler.SaveUsersToCsv(users);
                Console.WriteLine("Logging out...");
                inMenu = false;
                activeUser = null;
                break;

            default:
                Utils.DisplayAlertText("Invalid option, please try again.");
                break;
        }

    }

}



