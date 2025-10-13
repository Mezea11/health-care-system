namespace App
{
    class User : IUser
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        private Role role;
        private Registration registration;

        public List<Permissions> GrantedPermissions { get; private set; }

        public User(string username, string password, Role role)
        {
            Username = username;
            Password = password;
            this.role = role;
            registration = Registration.Pending;
            GrantedPermissions = new List<Permissions> { Permissions.None };
        }


        public Role GetRole() => role;
        public Registration GetRegistration() => registration;

        public bool TryLogin(string username, string password)
            => Username == username && Password == password;

        public void AcceptPending() => registration = Registration.Accepted;
        public void DenyPending() => registration = Registration.Denied;

        public void AcceptAddLocationPermission()
        {
            if (!GrantedPermissions.Contains(Permissions.AddLocation))
                GrantedPermissions.Add(Permissions.AddLocation);
        }

        public void DenyAddLocationPermission()
        {
            GrantedPermissions.Remove(Permissions.AddLocation);
            if (GrantedPermissions.Count == 0)
                GrantedPermissions.Add(Permissions.None);
        }

        public void AcceptAddRegistrationsPermission()
        {
            if (!GrantedPermissions.Contains(Permissions.AddRegistrations))
                GrantedPermissions.Add(Permissions.AddRegistrations);
        }

        public void DenyAddRegistrationsPermission()
        {
            GrantedPermissions.Remove(Permissions.AddRegistrations);
            if (GrantedPermissions.Count == 0)
                GrantedPermissions.Add(Permissions.None);
        }

        public bool HasPermission(string permissionName)
        {
            if (Enum.TryParse<Permissions>(permissionName, true, out var perm))
                return GrantedPermissions.Contains(perm);

            return false;
        }

        public override string ToString()
            => $"Username: {Username}, Role: {role}, Registration: {registration}, Permissions: {string.Join(", ", GrantedPermissions)}";
    }
}
