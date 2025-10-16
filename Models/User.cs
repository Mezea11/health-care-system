namespace App
{
    class User : IUser
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int Id { get; private set; }
        private Role role;
        private Registration registration;

        public List<Permissions> PermissionList { get; private set; }

        public User(int id, string username, string password, Role role)
        {
            Username = username;
            Password = password;
            this.role = role;
            Id = id;

            registration = (role == Role.Patient || role == Role.Admin) ? Registration.Pending : Registration.Accepted;

            PermissionList = new List<Permissions> { Permissions.None };
        }

        public Role GetRole() => role;
        public Registration GetRegistration() => registration;


        public bool TryLogin(string username, string password)
            => Username == username && Password == password;

        public void AcceptPending() => registration = Registration.Accepted;
        public void DenyPending() => registration = Registration.Denied;

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

        public bool HasPermission(string permissionName)
        {
            if (Enum.TryParse<Permissions>(permissionName, true, out var perm))
                return PermissionList.Contains(perm);

            return false;
        }

        public override string ToString()
            => $"Username: {Username}, Role: {role}, Registration: {registration}, Permissions: {string.Join(", ", PermissionList)}";

        private Region? assignedRegion = null;
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
