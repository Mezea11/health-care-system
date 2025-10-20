namespace App
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string RoleDetails { get; set; } = string.Empty;
        public Role Role { get; set; }
        public PersonellRoles PersonelRole { get; set; }
        public Registration Registration { get; set; }

        public List<Permissions> PermissionList { get; set; } = new List<Permissions> { Permissions.None };
        public List<int> AssignedPersonnelIds { get; set; } = new List<int>();
        // Konstruktor för nya användare
        public User(int id, string username, string password, Role role)
        {
            Id = id;

            Registration = (role == Role.Patient || role == Role.Admin) ? Registration.Pending : Registration.Accepted;
            Username = username;
            Role = role;
            var (hash, salt) = PasswordHelper.HashPassword(password);
            PasswordHash = hash;
            PasswordSalt = salt;

            Registration = (role == Role.Patient || role == Role.Admin)
                ? Registration.Pending
                : Registration.Accepted;

        }

        public bool AssignPersonnel(int personnelId)
        {
            // Kontrollerar att användaren är en Patient OCH att ID:t inte redan finns
            if (this.Role == Role.Patient && !AssignedPersonnelIds.Contains(personnelId))
            {
                AssignedPersonnelIds.Add(personnelId);
                return true; // Tilldelning lyckades
            }
            return false; // Tilldelning misslyckades (inte en patient eller redan tilldelad)
        }

        public void SetRolePersonell(int handleRole, IUser persObj, string roleDetails)
        {
            // Kontrollera om användaren är Personal innan vi ens försöker ändra något
            if (persObj.GetRole() == Role.Personnel)
            {
                // Kontrollera att det inkommande numret är giltigt
                if (Enum.IsDefined(typeof(PersonellRoles), handleRole))
                {
                    // Sätt den nya PersonelRolen
                    PersonelRole = (PersonellRoles)handleRole;

                    // Kontrollera om den NYA rollen är DOCTOR.
                    if (PersonelRole == PersonellRoles.Doctor)
                    {
                        // Om rollen ÄR DOCTOR:
                        // Sätt RoleDetails till det inmatade värdet om det är meningsfullt.
                        // Annars (om null, tom sträng, bara mellanslag), sätts det till string.Empty.
                        this.RoleDetails = string.IsNullOrWhiteSpace(roleDetails) ? string.Empty : roleDetails;
                    }
                    else
                    {
                        // Om rollen ÄR PERSONAL men INTE DOCTOR (t.ex. Nurse/Administrator):
                        // RoleDetails ska vara string.Empty. Detta rensar gamla Doctor-beskrivningar
                        // när en Doctor blir Nurse.
                        this.RoleDetails = string.Empty;
                    }
                }
                else
                {
                    // Ogiltigt nummer: Skriv ut fel, men ändra inte PersonelRole.
                    Console.WriteLine($"Värdet {handleRole} är inte en giltig personalroll.");

                    // Vi rensar RoleDetails här också, eftersom PersonelRole troligen inte ändrades
                    // men vi vet inte vilken den gamla var, så det är säkrare.
                    this.RoleDetails = string.Empty;
                }
            }
            else
            {
                // Om persObj.GetRole() INTE är Role.Personnel, ska varken PersonelRole
                // eller RoleDetails ändras från deras standardvärden (som är None / string.Empty).
                // Eftersom inga ändringar görs här, behåller de sina befintliga värden.
                // Det är inte nödvändigt att skriva kod här, men det är bra att vara medveten om.
            }
        }


        // Parameterlös konstruktor för JSON
        public User() { }
        public Role GetRole() => Role;

        public Registration GetRegistration() => Registration;

        public bool TryLogin(string username, string password)
            => Username == username &&
               PasswordHelper.VerifyPassword(password, PasswordHash, PasswordSalt);


        public void setRolePersonell()
        {
            PermissionList.Remove(Permissions.AddPersonell);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public void AcceptViewPermissions(Permissions perm)
        {
            if (!PermissionList.Contains(perm))
                PermissionList.Add(perm);
        }

        public void RevokePermission()
        {
            PermissionList.Remove(Permissions.AddLocation);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public void AcceptAddLocationPermission()
        {
            if (!PermissionList.Contains(Permissions.AddLocation))
                PermissionList.Add(Permissions.AddLocation);
        }

        public void DenyAddLocationPermission()
        {
            PermissionList.Remove(Permissions.AddLocation);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public void AcceptAddRegistrationsPermission()
        {
            if (!PermissionList.Contains(Permissions.AddRegistrations))
                PermissionList.Add(Permissions.AddRegistrations);
        }

        public void DenyAddRegistrationsPermission()
        {
            PermissionList.Remove(Permissions.AddRegistrations);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public void AcceptAddPersonellPermission()
        {
            if (!PermissionList.Contains(Permissions.AddPersonell))
                PermissionList.Add(Permissions.AddPersonell);
        }

        public void DenyAddPersonellPermission()
        {
            PermissionList.Remove(Permissions.AddPersonell);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public void AcceptViewPermissions()
        {
            if (!PermissionList.Contains(Permissions.ViewPermissions))
                PermissionList.Add(Permissions.ViewPermissions);
        }

        public void DenyViewPermissions()
        {
            PermissionList.Remove(Permissions.ViewPermissions);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        // public bool HasPermission(string permissionName)
        // {
        //     if (Enum.TryParse<Permissions>(permissionName, true, out var perm))
        //         return PermissionList.Contains(perm);

        //     return false;
        // }

        public bool HasPermission(Permissions permission)
            => PermissionList.Contains(permission);

        // public bool HasPermission(string permissionName)
        //     => Enum.TryParse<Permissions>(permissionName, true, out var perm) &&
        //        PermissionList.Contains(perm);

        public string ToPersonnelDisplay()
        {
            // Vi lägger till säkerhetskontroller för att vara defensiva
            if (Role != Role.Personnel || PersonelRole != PersonellRoles.Doctor)
            {
                // Returnera en tom sträng eller en standardsträng om det inte är en doktor
                return $"{Username} - No doctore.";
            }

            // Formatera som "Dr. [Username] - [RoleDetails]"
            // Använder ?? för att ersätta null/tom RoleDetails med "Ospecificerat"
            string details = string.IsNullOrWhiteSpace(RoleDetails) ? "Ospecificerat" : RoleDetails;

            return $"Dr. {Username} - {details}";
        }

        public override string ToString()
        {
            // Bygg strängen dynamiskt för att bara visa personalinformation om rollen är Personnel.
            string roleInfo = (Role == Role.Personnel)
                ? $", Personell Role: {PersonelRole}, Details: {RoleDetails}"
                : string.Empty;

            return $"ID: {Id}, Username: {Username}, Role: {Role}, Registration: {Registration}{roleInfo}, Roles as Personel: {PersonelRole}, Permissions: {string.Join(", ", PermissionList)}";
        }
        public Region? assignedRegion = null;
        public void AssignRegion(Region region)
        {
            assignedRegion = region;
        }
        public Region? GetAssignedRegion()
        {
            return assignedRegion;
        }

    }
}
