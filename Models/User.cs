namespace App;

// ============================
// Main class for users extends IUser interface
// ============================
class User : IUser
{
    public string Username { get; private set; }
    public int Id { get; private set; }

    // SET PASSWORD TO PRIVATE
    public string Password { get; private set; }

    private Role Role;
    public Registration RegistrationPending;

    public User(int id, string username, string password, Role role)
    {
        Id = id;
        Username = username;
        Password = password;
        Role = role;
        RegistrationPending = Registration.Accepted;


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
        RegistrationPending = Registration.Accepted;
    }

    public void DenyPending()
    {
        RegistrationPending = Registration.Denied;
    }
    public Registration GetRegistration() => RegistrationPending;
}
