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

        // === Ny metod: tilldela rättigheter baserat på roll ===
        // public void ApplyRolePermissions()
        // {
        //     if (RolePermissions.Map.TryGetValue(Role, out var perms))
        //         PermissionList = new List<Permissions>(perms);
        //     else
        //         PermissionList = new List<Permissions> { Permissions.None };
        // }

        // === Interface-krav ===
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

        public void AcceptViewPermissions()
        {
            if (!PermissionList.Contains(perm))
                PermissionList.Add(perm);
        }

        public void RevokePermission(Permissions perm)
        {
            PermissionList.Remove(perm);
            if (PermissionList.Count == 0)
                PermissionList.Add(Permissions.None);
        }

        public bool HasPermission(string permissionName)
        {
            if (Enum.TryParse<Permissions>(permissionName, true, out var perm))
                return PermissionList.Contains(perm);

            return false;
        }

        public bool HasPermission(Permissions permission)
            => PermissionList.Contains(permission);

        // public bool HasPermission(string permissionName)
        //     => Enum.TryParse<Permissions>(permissionName, true, out var perm) &&
        //        PermissionList.Contains(perm);

        public override string ToString()
            => $"ID: {Id}, Username: {Username}, Role: {Role}, Registration: {Registration}, Roles as Personel: {PersonelRole} Permissions: {string.Join(", ", PermissionList)}";
    }
}
