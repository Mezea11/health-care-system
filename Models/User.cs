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


        public void SetRolePersonell(int handleRole)
        {
            Console.WriteLine(handleRole);
            // Kontrollera att användaren faktiskt är "Personnel" innan du sätter en specifik roll
            // if (Role == Role.Personnel)
            // {
            //     PersonelRole = role;
            //     // Här kan du eventuellt lägga till logik för att uppdatera rättigheter baserat på den nya PersonellRoles
            //     // T.ex. ApplyPersonellRolePermissions();
            // }
            // else
            // {
            //     // Valfri logik: kasta ett undantag eller logga om man försöker sätta en personalroll på en icke-personal användare.
            //     // Exempel: throw new InvalidOperationException("Kan inte sätta personalroll på en icke-personal användare.");
            // }
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

        }
        public void AcceptPending() => Registration = Registration.Accepted;
        public void DenyPending() => Registration = Registration.Denied;

        public void GrantPermission(Permissions perm)
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
            => $"ID: {Id}, Username: {Username}, Role: {Role}, Registration: {Registration}, Permissions: {string.Join(", ", PermissionList)}";
    }
}
