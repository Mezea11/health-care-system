namespace App
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public Role Role { get; set; }
        public PersonellRoles PersonelRole { get; set; }
        public Registration Registration { get; set; }

        public List<Permissions> PermissionList { get; set; } = new List<Permissions> { Permissions.None };

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

            // ApplyRolePermissions();
        }


        public void SetRolePersonell(int handleRole, IUser persObj)
        {
            if (persObj.GetRole() == Role.Personnel)
            {
                // Kontrollera att användaren faktiskt är "Personnel" innan du sätter en specifik roll
                if (Enum.IsDefined(typeof(PersonellRoles), handleRole))
                {
                    this.PersonelRole = (PersonellRoles)handleRole;
                }
                else
                {
                    // Valfri hantering för ett ogiltigt nummer
                    Console.WriteLine($"Värdet {handleRole} är inte en giltig personalroll.");
                }
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

        // public void AcceptViewPermissions()
        // {
        //     if (!PermissionList.Contains(perm))
        //         PermissionList.Add(perm);
        // }

        public void RevokePermission(Permissions perm)
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

        // DONT REMOVE >YOURSELF 

        public void AcceptPending() => Registration = Registration.Accepted;
        public void DenyPending() => Registration = Registration.Denied;

        public void GrantPermission(Permissions perm)
        {
            if (!PermissionList.Contains(perm))
                PermissionList.Add(perm);
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

        public override string ToString()
            => $"ID: {Id}, Username: {Username}, Role: {Role}, Registration: {Registration}, Roles as Personel: {PersonelRole} Permissions: {string.Join(", ", PermissionList)}";

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
