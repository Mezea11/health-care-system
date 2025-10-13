namespace App;

// ============================
// Main class for users extends IUser interface
// ============================
class User : IUser
{
    public string Username { get; private set; }

    // SET PASSWORD TO PRIVATE
    public string Password { get; private set; }

    private Role Role;
    public Registration RegistrationPending;
    public Permissions PermissionPending;
    public List<Permissions> PermissionList { get; private set; }

    public User(string username, string password, Role role)
    {
        Username = username;
        Password = password;
        Role = role;
        RegistrationPending = Registration.Pending;
        PermissionPending = Permissions.None;
        PermissionList = new List<Permissions> { Permissions.None };
    }

    public Role GetRole() => Role;
    // public Registration GetRegistration => RegistrationPending;

    // Try login method
    public bool TryLogin(string username, string password)
    {
        return Username == username && Password == password;
    }

    public void AcceptAddRegistrationsPermission()
    {
        if (!PermissionList.Contains(Permissions.AddRegistrations))
            PermissionList.Add(Permissions.AddRegistrations);
    }

    public void DenyAddRegistrationsPermission()
    {
        PermissionList.Remove(Permissions.AddRegistrations);

        // Om inga rättigheter finns kvar, sätt None
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

        // Om inga rättigheter finns kvar, sätt None
        if (PermissionList.Count == 0)
            PermissionList.Add(Permissions.None);
    }


    public void AcceptPending()
    {
        RegistrationPending = Registration.Accepted;
    }

    public void DenyPending()
    {
        RegistrationPending = Registration.Denied;
    }

    public void AcceptAddLocationPermission()
    {
        PermissionPending = Permissions.AddLocation;
    }

    public void DenyAddLocationPermission()
    {
        PermissionPending = Permissions.None;
    }

    public override string ToString()
    {
        return "Username: " + Username;
    }

    public Permissions GetPermissions() => PermissionPending;

    public Registration GetRegistration() => RegistrationPending;

}


