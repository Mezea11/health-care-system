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

    public User(string username, string password, Role role)
    {
        Username = username;
        Password = password;
        Role = role;
        RegistrationPending = Registration.Pending;
    }

    public Role GetRole() => Role;
    // public Registration GetRegistration => RegistrationPending;

    // Try login method
    public bool TryLogin(string username, string password)
    {
        return Username == username && Password == password;
    }

    public void AcceptPending()
    {
        RegistrationPending = Registration.Accept;
    }

    public void DenyPending()
    {
        RegistrationPending = Registration.Deny;
    }
    public Registration GetRegistration() => RegistrationPending;
}
