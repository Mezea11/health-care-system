namespace App;


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

class FileHandler
{
    public static string UserFileName = "data/users.csv";

    private const char PrimarySeperator = '|';
    private const char ListSeperator = ',';
    const string DefaultPassword = "123";
    private static List<IUser> CreateTestUser()
    {
        Console.WriteLine("file dont exist");
        const string DefaultPassword = "123";

        // 1. Skapa alla användare med den enkla konstruktorn
        // Detta hanterar ID, Username, Role, och att skicka in password som has/saltar det 
        var users = new List<User>
    {
        // Enkla roller
        new User(0, "patient", DefaultPassword, Role.Patient),
        new User(1, "personell", DefaultPassword, Role.Personnel),
        new User(2, "admin", DefaultPassword, Role.Admin),
        new User(3, "admin", DefaultPassword, Role.Admin),
        new User(4, "admin1", DefaultPassword, Role.Admin),
        new User(5, "admin2", DefaultPassword, Role.Admin),

        // Läkare skapas först som vanlig 'Personnel'
        new User(6, "Smith", DefaultPassword, Role.Personnel),
        new User(7, "admin_doctor", DefaultPassword, Role.Personnel),
        new User(8, "Andersson", DefaultPassword, Role.Personnel),
        new User(9, "Snuggles", DefaultPassword, Role.Personnel),
        new User(10, "Livingstone", DefaultPassword, Role.Personnel),
        new User(11, "superadmin", DefaultPassword, Role.SuperAdmin),
    };

        // ----------------------------------------------------------------------
        // 2. HANTERING AV BEHÖRIGHETER (Permissions)
        // ----------------------------------------------------------------------

        // Hitta admin med ID 2 (första admin som får permissons för att göra saker)
        var mainAdmin = users.FirstOrDefault(u => u.Id == 2);
        if (mainAdmin != null)
        {
            // Tänk: mainAdmin.PermissionList.Add(Permissions.ManageUsers);
            mainAdmin.PermissionList.Add(Permissions.AddPersonell);
            mainAdmin.PermissionList.Add(Permissions.AddRegistrations);
            mainAdmin.Registration = Registration.Accepted;
        }

        // Hitta admin med ID 4
        var secondaryAdmin = users.FirstOrDefault(u => u.Id == 4);
        secondaryAdmin?.PermissionList.Add(Permissions.AddPersonell);

        // ----------------------------------------------------------------------
        // 3. LÄKARHANTERING (Använd LINQ (FirstOrDefault) för att hitta och uppdatera specifika användare)
        // ----------------------------------------------------------------------
        // 

        // Smith (Doctor, Cardiology)
        var smith = users.FirstOrDefault(u => u.Id == 6);
        if (smith != null)
        {
            smith.PersonelRole = PersonellRoles.Doctor;
            smith.RoleDetails = "Cardiology";
        }

        // admin_doctor (Doctor, General Medicine)
        var adminDoctor = users.FirstOrDefault(u => u.Id == 7);
        if (adminDoctor != null)
        {
            adminDoctor.PersonelRole = PersonellRoles.Doctor;
            adminDoctor.RoleDetails = "General Medicine";
        }

        // Andersson (Doctor, Neurology)
        var andersson = users.FirstOrDefault(u => u.Id == 8);
        if (andersson != null)
        {
            andersson.PersonelRole = PersonellRoles.Doctor;
            andersson.RoleDetails = "Neurology";
        }

        // Snuggles (Doctor, Metaphysical Wellness)
        var snuggles = users.FirstOrDefault(u => u.Id == 9);
        if (snuggles != null)
        {
            snuggles.PersonelRole = PersonellRoles.Doctor;
            snuggles.RoleDetails = "Metaphysical Wellness";
        }

        // Livingstone (Doctor, complex RoleDetails)
        var livingstone = users.FirstOrDefault(u => u.Id == 10);
        if (livingstone != null)
        {
            livingstone.PersonelRole = PersonellRoles.Doctor;
            livingstone.RoleDetails = "medicine, geography, and exploration";
        }
        // spara till data.csv 
        SaveUsersToCsv(users.ConvertAll<IUser>(u => u));
        // 3. Returnera listan som List<IUser>
        return users.ConvertAll<IUser>(u => u);
    }

    // === ERSÄTTNING FÖR SerializePermissions ===
    /// <summary>
    /// Konverterar en lista med Permissions till en sträng, separerad av ListSeparator.
    /// Tar bort Permissions.None om den finns.
    /// </summary>
    private static string SerializePermissions(List<Permissions> permissions)
    {
        var validPermissions = new List<string>();

        foreach (var perm in permissions)
        {
            // Kontrollera om behörigheten INTE är Permissions.None
            if (perm != Permissions.None)
            {
                validPermissions.Add(perm.ToString());
            }
        }

        // Sammanfoga listan av strängar till en enda sträng
        return string.Join(ListSeperator, validPermissions);
    }

    // === ERSÄTTNING FÖR SerializeIntList ===
    /// <summary>
    /// Konverterar en lista med int-ID:n till en sträng, separerad av ListSeparator.
    /// </summary>
    private static string SerializeIntList(List<int> ids)
    {
        var idStrings = new List<string>();

        foreach (var id in ids)
        {
            // Konvertera varje int till en sträng
            idStrings.Add(id.ToString());
        }

        // Sammanfoga listan av strängar till en enda sträng
        return string.Join(ListSeperator, idStrings);
    }


    /// <summary>
    /// Laddar användare från CSV-filen.
    /// </summary>

    public static List<IUser> LoadFromCsv()
    {
        if (!File.Exists(UserFileName))
        {
            return CreateTestUser();
        }

        List<IUser> loadedUsers = new List<IUser>();

        try
        {
            // hämta all data från filen
            string[] lines = File.ReadAllLines(UserFileName);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // vi börjar med att dela upp raden i fält med PrimarySeparator
                string[] parts = line.Split(PrimarySeperator);

                // Här förväntar vi oss 10 fält
                // Id, Username, PasswordHash, PasswordSalt, Role, PersonelRole, RoleDetails, Registration, PermissionList, AssignedPersonnelIds
                if (parts.Length == 10)
                {
                    User user = new User();


                    // börjar att populera enkla fält och konvertering (int, string och enum)
                    user.Id = int.Parse(parts[0]);
                    user.Username = parts[1];
                    user.PasswordHash = parts[2];
                    user.PasswordSalt = parts[3];
                    user.Role = (Role)Enum.Parse(typeof(Role), parts[4], true);
                    user.PersonelRole = (PersonellRoles)Enum.Parse(typeof(PersonellRoles), parts[5], true);
                    user.RoleDetails = parts[6];
                    user.Registration = (Registration)Enum.Parse(typeof(Registration), parts[7], true);

                    // Deseralisering av PermissionList
                    string permissionString = parts[8];
                    user.PermissionList = new List<Permissions>();
                    if (!string.IsNullOrWhiteSpace(permissionString))
                    {
                        string[] perms = permissionString.Split(ListSeperator);

                        foreach (string permStr in perms)
                        {
                            if (Enum.TryParse<Permissions>(permStr.Trim(), true, out Permissions perm))
                            {
                                user.PermissionList.Add(perm);
                            }

                        }
                    }

                    // Vi kollar för säkerhetsskull att listan inte är tom. Är den tom så sätter vi Permission none för den usern 
                    if (user.PermissionList.Count == 0) user.PermissionList.Add(Permissions.None);


                    // Descentralisering av AssignedPersonalId 
                    string assignedIdString = parts[9];
                    user.AssignedPersonnelIds = new List<int>();
                    if (!string.IsNullOrWhiteSpace(assignedIdString))
                    {
                        string[] idStrings = assignedIdString.Split(ListSeperator);
                        foreach (string IdString in idStrings)
                        {
                            if (int.TryParse(IdString.Trim(), out int id)) user.AssignedPersonnelIds.Add(id);
                        }
                    }

                    loadedUsers.Add(user);
                }

            }
            return loadedUsers;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR loading users from CSV: {ex.Message}");
            return new List<IUser>();
        }


    }



    /// <summary>
    /// Sparar en lista med användare till CSV-filen.
    /// </summary>
    public static void SaveUsersToCsv(List<IUser> users)
    {

        List<string> lines = new List<string>();

        // Förbered konvertering av listor till strängar:
        // Vi tar bort Permissions.None från listan innan serialisering, 
        // eftersom den läggs till igen vid deserialisering om listan är tom.
        // Detta gör filen renare.
        // Mysigt med hanering av funktioner med linq på detta sättet, Men jag skrev dem som egna funktioner ovan.
        // Func<List<Permissions>, string> SerializePermissions = perms => string.Join(ListSeperator, perms.Where(p => p != Permissions.None));

        // Func<List<int>, string> SerializeIntList = ids => string.Join(ListSeperator, ids);

        foreach (User user in users)
        {
            if (user is User concreteUser)
            {
                string permissionListString = SerializePermissions(concreteUser.PermissionList);
                string assignedIdsString = SerializeIntList(concreteUser.AssignedPersonnelIds);


                // skapa raden med PrimarySeaparator | 
                string line = string.Join(PrimarySeperator,
                concreteUser.Id,
                concreteUser.Username,
                concreteUser.PasswordHash,
                concreteUser.PasswordSalt,
                concreteUser.Role,
                concreteUser.PersonelRole,
                concreteUser.RoleDetails,
                concreteUser.Registration,
                permissionListString,  // seraliserar första listan
                assignedIdsString // seraliserar andra listan
                );

                lines.Add(line);
            }
        }

        try
        {
            File.WriteAllLines(UserFileName, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR saving users to CSV: {ex.Message}");
        }
    }


}